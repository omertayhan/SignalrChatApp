﻿using ChatRooms.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignalrChatApp.Common;
using SignalrChatApp.Data;
using SignalrChatApp.Models;
using SignalrChatApp.ViewModels;
using System.Diagnostics;

namespace SignalrChatApp.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly AppDbContext _dbContext;

    public HomeController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IActionResult> Index()
    {
        var messages = await _dbContext.GetMessagesAsync();

        // Mesajları alırken MessagesVM türüne dönüştür
        var messagesVM = messages.Select(m => new MessagesVM
        {
            MessageId = m.MessageId,
            Message = CryptoHelper.DecryptString(m.Message),
            MessageType = m.MessageType,
            MessageGroupId = m.MessageGroupId,
            SenderUser = m.SenderUser,
            ReceiverUser = m.ReceiverUser,
            CreatedTime = m.CreatedTime
        }).ToList();

        return View(messagesVM);
    }

    [HttpPost]
    public async Task<IActionResult> SaveMessage([FromBody] MessagesVM model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var message = new Messages
            {
                MessageId = Guid.NewGuid(),
                Message = CryptoHelper.EncryptString(model.Message),
                MessageType = MessageTypes.Public.ToString(),
                MessageGroupId = null,
                SenderUser = model.SenderUser,
                ReceiverUser = null,
                CreatedTime = model.CreatedTime
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception)
        {

            return StatusCode(500, "Internal server error");
        }
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}