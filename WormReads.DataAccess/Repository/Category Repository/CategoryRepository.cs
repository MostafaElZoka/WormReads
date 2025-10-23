﻿using WormReads.Data;
using WormReads.Models;

namespace WormReads.DataAccess.Repository.Category_Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository 
{ 
    private readonly AppDbContext _dbContext;
    public CategoryRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }



    public void Update(Category category)
    {
        _dbContext.Categories.Update(category);
    }
}
