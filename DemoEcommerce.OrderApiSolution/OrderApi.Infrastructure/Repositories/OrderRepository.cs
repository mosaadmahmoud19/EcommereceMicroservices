﻿using eCommerece.SharedLibrary.Logs;
using eCommerece.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
                return order.Id > 0 ? new Response(true , "OrdervPlaced Successfully"):
                    new Response(false, "Error occured While Placing order");

            }catch(Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false,"Error Ocure While Placing order");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order is null)
                {
                    return new Response(false, "Order not found");
                }

                context.Orders.Remove(entity);

                await context.SaveChangesAsync();

                return new Response(true, "Order Successful deleted");

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Error Ocure While deleting order");
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
                LogException.LogExceptions(ex);

                throw new Exception("Error occured while retrieving order");
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {

            try
            {
              var orders =await context.Orders.AsNoTracking().ToListAsync();

                return orders is not null ? orders : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occured while placing order");
            }

        }

        public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.Orders.Where(predicate).FirstOrDefaultAsync();
                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occured while retrieving order");
            }
        }

        public async Task<IEnumerable<Order>> GetOrderAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.Orders.Where(predicate).ToListAsync();
                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occured while placing order");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {

            try 
            {
                var order = await FindByIdAsync(entity.Id);
               

                if (order is null)
                {
                    return new Response(false, $"Order not found");
                }

                context.Entry(order).State = EntityState.Detached;

                context.Orders.Update(entity);

                await context.SaveChangesAsync();

                return new Response(true, "Order Updated");
            }
            catch(Exception ex) 
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Error Ocure While deleting order");
            }

               
         }
          
            
        
        
    }
}
