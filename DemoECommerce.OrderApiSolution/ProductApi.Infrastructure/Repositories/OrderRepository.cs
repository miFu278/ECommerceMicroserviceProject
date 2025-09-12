using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                var order = context.Orders.Add(entity).Entity;
                await context.SaveChangesAsync();
                return order.Id > 0 ? new Response(true, "Order placed successfully") :
                    new Response(false, "Error occrued while placing order");
            }
            catch (Exception ex)
            {
                // log original exception
                LogException.LogExceptions(ex);

                // display scary-free message to client
                return new Response(false, "Error occured while placing order");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order is null)
                    return new Response(false, "Order not found");

                context.Orders.Remove(entity);
                await context.SaveChangesAsync();
                return new Response(true, "Order successfully deleted");
            }
            catch (Exception ex)
            {
                // log original exception
                LogException.LogExceptions(ex);

                // display scary-free message to client
                return new Response(false, "Error occured while deleting order");
            }

        }

        public async Task<Order> FindByIdAsync(int id)
        {
            try
            {
                var order = await context.Orders!.FindAsync(id);
                return order is not null ? order : null!;

            }
            catch (Exception ex)
            {
                // log original exception
                LogException.LogExceptions(ex);

                // display scary-free message to client
                throw new Exception("Error occured while deleting order");
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                var orders = await context.Orders.AsNoTracking().ToListAsync();
                return orders is not null ? orders : null!;
            }
            catch (Exception ex)
            {
                // log original exception
                LogException.LogExceptions(ex);

                // display scary-free message to client
                throw new Exception("Error occured while retrieving order");
            }
        }

        public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.Orders.Where(predicate).FirstOrDefaultAsync()!;
                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {
                // log original exception
                LogException.LogExceptions(ex);

                // display scary-free message to client
                throw new Exception("Error occured while retrieving order");
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var orders = await context.Orders.Where(predicate).ToListAsync();
                return orders is not null ? orders : null!;
            }
            catch (Exception ex)
            {
                // log original exception
                LogException.LogExceptions(ex);

                // display scary-free message to client
                throw new Exception("Error occured while placing order");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order is null)
                    return new Response(false, $"Order not found");

                context.Entry(order).State = EntityState.Modified;
                context.Orders.Update(entity);
                return new Response(true, "Order updated successfully");
            }
            catch (Exception ex)
            {
                // log original exception
                LogException.LogExceptions(ex);

                // display scary-free message to client
                return new Response(false, "Error occured while updating order");
            }
        }
    }
}
