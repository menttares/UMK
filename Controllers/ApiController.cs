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

[Authorize]
public class ApiController : Controller
{
    private readonly ILogger<ApiController> _logger;

    private Database _Database;
    public ApiController(ILogger<ApiController> logger, Database Database)
    {
        _logger = logger;
        _Database = Database;
    }

    [HttpGet]
    public IActionResult GetKnowledge(int Type)
    {

        var data = _Database.Knowledges.Where(k => k.TypeKnowledgeId == Type && k.IsAccess);
        // if (data is null)
        //     return NotFound();

        return Json(data);
    }

    [HttpGet]
    public IActionResult GetKnowledgeAll(string str)
    {
        Dictionary<int, List<Knowledge>> data = new();
        for (int typeId = 1; typeId <= 4; typeId++)
        {
            var find = _Database.Knowledges.Where(k => k.TypeKnowledgeId == typeId).Where(k => k.Name == str);
            if (find is null || find.Count() == 0)
                continue;

            data[typeId] = find.ToList();
        }

        return Json(data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> PostKnowledge(IFormFile file)
    {
        var Id = Request.Form["Id"];
        await _Database.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    /// <summary>
    /// Получения множества фала одной записи 
    /// </summary>
    /// <param name="Id">Номер записи, у которой получаем файлы</param>
    /// <returns></returns>
    public async Task<IActionResult> GetFiles(int Id)
    {

        var data = _Database.KnowledgesFiles.Where(kf => kf.Knowledge.Id == Id).Select(kf => kf.File);

        if (data is null)
            return NotFound("Файлы не найден");

        return Json(data);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete]
    public async Task<IActionResult> DeleteFile(int IdKnowledge, int IdFile)
    {
        _logger.LogInformation($"IdKnowledge: {IdKnowledge} IdFile:{IdFile}");
        var find = _Database.KnowledgesFiles.FirstOrDefault(kf => kf.KnowledgeId == IdKnowledge && kf.FileId == IdFile);
        var file = _Database.Files.FirstOrDefault(
            f => f.Id == find.FileId 
        );
        if (find is null)
            return BadRequest("Файл не найден");

        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files",file.Name);

        if (System.IO.File.Exists(filePath))
        {
            try
            {
                System.IO.File.Delete(filePath);
                return RedirectToAction("Index"); // перенаправление на страницу с таблицей файлов или на любую другую страницу
            }
            catch (IOException ex)
            {
                // обработать исключение, если файл заблокирован или недоступен
                _logger.LogError(ex, $"Ошибка при удалении файла ");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        _Database.KnowledgesFiles.Remove(find);
        await _Database.SaveChangesAsync();
        return Ok();
    }


    [HttpGet]
    public async Task<IActionResult> DownFile(string filename)
    {
        try
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", filename);

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            //Response.Headers.ContentDisposition = $"attachment; filename={filename}";
            await Response.SendFileAsync(filePath);
            return File(fileBytes, "application/octet-stream");

        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex.Message);

            throw;
        }

    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> PutKnowledge(int Id, string Text)
    {
        var find = _Database.Knowledges.FirstOrDefault(
            kn => kn.Id == Id
        );
        if (find is null)
            return BadRequest();

        find.Description = Text;
        await _Database.SaveChangesAsync();
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> PostFile(IFormFile file)
    {

        int Id = int.Parse(Request.Form["Id"]);

        if (file == null || file.Length == 0)
            return BadRequest("Пустой файл");

        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files");
        string fileName = Path.GetFileName(file.FileName);
        string filePath = Path.Combine(path, fileName);

        string extension = Path.GetExtension(fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        if (!System.IO.File.Exists(filePath))
            return BadRequest("Произошла ошибка сохранения файла");

        Models.File newFile = new() { Name = fileName, Type = extension };

        _Database.Files.Add(newFile);

        var findKnowledge = _Database.Knowledges.FirstOrDefault(kn => kn.Id == Id);
        if (findKnowledge is null)
            return BadRequest("Knowledge не найден");


        _Database.KnowledgesFiles.Add(new KnowledgeFiles() { Knowledge = findKnowledge, File = newFile });


        await _Database.SaveChangesAsync();

        return Json(newFile);
    }


    [HttpGet]
    public IActionResult GetTest(int Id)
    {
        var IsAccess = _Database.Knowledges.FirstOrDefault(k => k.Id == Id).IsAccess;
        if (!IsAccess)
            return NotFound();
        var test = _Database.Tests.SingleOrDefault(t => t.KnowledgeId == Id);
        if (test is null)
        {
            return NotFound();
        }

        var questions = _Database.TestsQuestioins
                                .Where(tq => tq.TestId == test.Id).Select(tq => tq.Questioin)
                                .ToList();

        List<QuestionsTest> questionsTest = new();
        foreach (var question in questions)
        {
            var options = _Database.OptionsQuestioins.Where(oq => oq.QuestioinId == question.Id)
                                                    .Select(oq => oq.Option).ToList();

            questionsTest.Add(new QuestionsTest(question, options));
        }
        TestResponse testResponse = new(test, questionsTest);

        return PartialView("Test", testResponse);
    }


    [HttpPost]
    public async Task<IActionResult> SendResultTest([FromBody] TestResultModel model)
    {
        var test = _Database.Tests.FirstOrDefault(t => t.Id == model.IdTest);

        var questions = _Database.TestsQuestioins
                                .Where(tq => tq.TestId == model.IdTest).Select(tq => tq.Questioin)
                                .ToList();

        int CountAnswers = questions.Count();
        double CountTryOptions = 0;
        foreach (var question in questions)
        {
            if (!model.Answers.Keys.Contains(question.Id))
                continue;

            var option = _Database.OptionsQuestioins.Where(oq => oq.QuestioinId == question.Id && oq.IsTry).Select(oq => oq.OptionId).ToList();
            if (option.Contains(model.Answers[question.Id]))
            {
                //option.Contains(model.Answers[question.Id]);

                CountTryOptions++;
            }
        }


        double mark = CountTryOptions / CountAnswers * 10;
        _logger.LogInformation($@"
        CountTryOptions{CountTryOptions},
        CountAnswers{CountAnswers},
        mark{mark},
        ");
        string formattedMark = mark.ToString("0.0");
        var data = new
        {
            Mark = formattedMark
        };

        var role = Request.Cookies["Role"];
        if (role == "User")
        {
            ResultTest resultTest = new();
            resultTest.NameStydent = Request.Cookies["Name"];
            resultTest.NameGroup = Request.Cookies["Group"];
            resultTest.Mark = (int)mark;
            resultTest.Test = test;

            _Database.ResultTests.Add(resultTest);
            await _Database.SaveChangesAsync();
        }

        return Json(data);
    }



    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult AdminPanel()
    {

        return PartialView();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult GetTable(int Id)
    {
        var data = _Database.Knowledges.Where(k => k.TypeKnowledgeId == Id)
                                        .ToList();
        if (data is null)
            return NotFound();

        ViewBag.TypeKnowledgeId = Id;
        return PartialView("BuildTable", data);
    }
}

public class TestResultModel
{
    public int IdTest { get; set; }
    public Dictionary<int, int> Answers { get; set; }
}