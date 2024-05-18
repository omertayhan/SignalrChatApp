"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl('/chatHub').build();

connection.on("Send", function (message) {
    $("#messageList").append(`<li class="list-group-item">${message}</li>`);
});

connection.on("ReceiveMessage", (u, m) => {
    $("#messageList").append(`<li class="list-group-item"><span>${u}</span>: ${m}</li>`);
});

document.getElementById("groupmsg").addEventListener("click", async (event) => {
    var groupName = document.getElementById("group-name").value;
    var userName = document.getElementById("user-name").value;
    var groupMsg = document.getElementById("group-message-text").value;
    try {
        connection.invoke("SendMessageToGroup", groupName, groupMsg, userName);
    }
    catch (e) {
        console.error(e.toString());
    }
    event.preventDefault();
});

document.getElementById("join-group").addEventListener("click", async (event) => {
    var groupName = document.getElementById("group-name").value;
    var userName = document.getElementById("user-name").value;
    try {
        connection.invoke("AddToGroup", groupName, userName);
    }
    catch (e) {
        console.error(e.toString());
    }
    event.preventDefault();
});

document.getElementById("leave-group").addEventListener("click", async (event) => {
    var groupName = document.getElementById("group-name").value;
    var userName = document.getElementById("user-name").value;
    try {
        connection.invoke("RemoveFromGroup", groupName, userName);
    }
    catch (e) {
        console.error(e.toString());
    }
    event.preventDefault();
});
connection.start().then(() => {
    
});