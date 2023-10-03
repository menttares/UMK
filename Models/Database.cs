using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace УМК.Models;

public class Database : DbContext
{

    public DbSet<Account> Accounts { get; set; }

    public DbSet<Knowledge> Knowledges { get; set; }

    public DbSet<File> Files { get; set; }

    public DbSet<KnowledgeFiles> KnowledgesFiles { get; set; }

    public DbSet<Questioin> Questioins { get; set; }

    public DbSet<Option> Options { get; set; }

    public DbSet<OptionsQuestioin> OptionsQuestioins { get; set; }

    public DbSet<TestQuestioins> TestsQuestioins { get; set; }

    public DbSet<ResultTest> ResultTests { get; set; }

    public DbSet<Test> Tests { get; set; }

    public DbSet<TypeKnowledge> TypeKnowledges { get; set; }
    

    public Database(DbContextOptions<Database> options)
        : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // определение сущностей и свойств
        
        // добавление начальных данных
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder = null)
    {
        // добавление начальных данных в базу данных
        var accounts = new List<Account>()
        {
            new Account() { Id = 1, Name = "Администратор", Email = "Admin@gmail.com", Password="Admin" },
        };

        if (modelBuilder != null)
        {
            // с помощью modelBuilder.AddEntity() добавляем начальные данные в модель базы данных
            modelBuilder.Entity<Account>().HasData(accounts);
        }
        else
        {
            // если modelBuilder не передан, то используем контекст
            Accounts.AddRange(accounts);
            SaveChanges();
        }
    }

}