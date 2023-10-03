using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using УМК.Models;


namespace УМК.Controllers;

[Authorize(Roles = "Admin")]
public class AdminPanelController : Controller
{
    private readonly ILogger<AdminPanelController> _logger;

    private Database _Database;

    public AdminPanelController(ILogger<AdminPanelController> logger, Database Database)
    {
        _logger = logger;
        _Database = Database;
    }


    [HttpPut]
    public async Task<IActionResult> PutTitleKnowledge(int Id, string Title)
    {
        var find = _Database.Knowledges.FirstOrDefault(
            kn => kn.Id == Id
        );
        if (find is null)
            return BadRequest();

        find.Name = Title;
        await _Database.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetResultTests()
    {
        Dictionary<ResultTest, string> results = new();
        foreach (var result in _Database.ResultTests.ToList())
        {
            results.Add(result, _Database.Tests.FirstOrDefault(t => t.Id == result.TestId)?.Name ?? "none");
        }


        if (results is null)
            return NotFound();

        return PartialView("ResultTests", results);
    }

    [HttpPost]
    public async Task<IActionResult> PostChangeKnowledge(int Id, string Name, string Description, bool IsAccess)
    {
        var find = _Database.Knowledges.FirstOrDefault(
            k => k.Id == Id
        );
        if (find is null)
            return BadRequest();

        if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Description))
            return BadRequest();

        find.Name = Name;
        find.Description = Description;
        find.IsAccess = IsAccess;
        await _Database.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteKnowledge(int Id)
    {
        var find = _Database.Knowledges.FirstOrDefault(
            k => k.Id == Id
        );

        if (find is null)
            return BadRequest();

        _Database.Knowledges.Remove(find);
        await _Database.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> PostKnowledge(int Type)
    {

        var type = _Database.TypeKnowledges.FirstOrDefault(
            tk => tk.Id == Type
        );

        if (type is null)
            return BadRequest();

        Knowledge newKnowledge = new()
        {
            Name = "New",
            Description = "New",
            IsAccess = false,
            TypeKnowledge = type
        };

        _Database.Knowledges.Add(newKnowledge);
        await _Database.SaveChangesAsync();

        return Json(newKnowledge);
    }


    [HttpGet]
    public async Task<IActionResult> GetExcelForm()
    {

        return PartialView("ExcelForm");
    }

    [HttpPost]
    public async Task<IActionResult> PostExcelForm(IFormFile file)
    {
        if (file != null && Path.GetExtension(file.FileName) == ".xlsx" && (file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || file.ContentType == "application/vnd.ms-excel"))
        {
            using (var excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = excelPackage.Workbook.Worksheets.First();

                var rows = worksheet.Dimension.Rows;

                List<Knowledge> knowledgeExcel = new(); 
                // Проходим по строкам в Excel файле и обрабатываем их
                for (int i = 2; i <= rows; i++) // Начинаем со 2-й строки, так как первая строка - заголовки столбцов
                {
                    var title = worksheet.Cells[i, 1].Value?.ToString();
                    var description = worksheet.Cells[i, 2].Value?.ToString();
                    var accessStr = worksheet.Cells[i, 3].Value?.ToString();
                    var Type = worksheet.Cells[i, 4].Value?.ToString();
                    
                    bool access = false;
                    
                    if (accessStr == "Да")
                        access = true;
                    
                    int IdType = 0;
                    switch (Type)
                    {
                        case "Лекции":
                            IdType = 1;
                        break;
                        case "ОКР":
                            IdType = 4;
                        break;
                        case "Лабораторные работы":
                            IdType = 2;
                        break;
                        default:
                        break;
                    }
                    
                    var typeKnowledge = _Database.TypeKnowledges.FirstOrDefault(
                        t => t.Id == IdType
                    ); 
                    if (typeKnowledge is null)
                        continue;
                    
                    knowledgeExcel.Add(
                        new Knowledge(){
                            Name = title,
                            Description = description,
                            IsAccess = access,
                            TypeKnowledge = typeKnowledge
                        }
                    );
                    
                    //_logger.LogInformation($"Title: {title}, Description: {description}");
                }

                _Database.Knowledges.AddRange(
                    knowledgeExcel
                );

                await _Database.SaveChangesAsync(true);

                return Content("Файл успешно загружен и обработан.");
            }
        }

        return Content("Ошибка загрузки файла. Убедитесь, что файл имеет расширение .xlsx и тип application/vnd.ms-excel.");

    }

}
