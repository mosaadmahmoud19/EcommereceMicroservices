﻿using eCommerece.SharedLibrary.Logs;
using eCommerece.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.infrastructure.Repositories
{
    public class ProductRepository (ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                // check if the product already exist

                var getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));

                    if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                    {

                        return new Response(false, $"{entity.Name} already added");
                    }                

                    var currentEntity = context.Products.Add(entity).Entity;
                    await context.SaveChangesAsync();

                    if (currentEntity is not null && currentEntity.Id > 0)
                    {

                        return new Response(true, $"{entity.Name} added to database successfully");
                    }


                    else 
                    {
                        return new Response(false, $" Error Occure While adding {entity.Name} ");
                    }





            }
            catch (Exception ex) 
            { 
               LogException.LogExceptions(ex);
                return new Response(false, $" Error Occure Adding New Product");

            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);

                if (product is null)
                {
                    return new Response(false, $"{entity.Name}  not found");

                }

                context.Products.Remove(product);
                await context.SaveChangesAsync();

                return new Response(true, $"{entity.Name}  is deleted  successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, $" Error Occure delete  Product");

            }



        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try 
            {
                var product = await context.Products.FindAsync(id);

                return product is not null ? product : null!;


            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error Occure reterving produt");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try 
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Error Occure reterving produt");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;

                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Error Occure reterving produt");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {

                var product = await FindByIdAsync(entity.Id);

                if (product is null)
                {
                    return new Response(false, $"{entity.Name}  not found");

                }

                context.Entry(product).State = EntityState.Detached;

                context.Products.Update(entity);

                await context.SaveChangesAsync();


                return new Response(true, $"{entity.Name}  is Update  successfully");
            }
            catch (Exception ex) 
            {
                LogException.LogExceptions(ex);
                return new Response(false, " Error Ocureed Updateing exsiting product");

            }

        }
    }
}
