"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl('/chatHub').build();

connection.on("Send", function (message) {
    var groupName = document.getElementById("group-name").value;
    var userName = document.getElementById("user-name").value;

    var currentDate = new Date();
    var timestamp = new Date(currentDate).toISOString();

    var formattedTimestamp = new Date(timestamp).toLocaleString('en-US', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: true
    });

    var offset = 3 * 60;
    var dbtimestamp = new Date(currentDate.getTime() + offset * 60000).toISOString();

    fetch("/GroupChat/SaveMessage", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            SenderUser: userName,
            message: message,
            MessageGroupId: groupName,
            CreatedTime: dbtimestamp
        })
    }).catch(function (err) {
        console.error(err.toString());
    });

    $("#messageList").append(`<li class="list-group-item"><small class="text-muted">${formattedTimestamp}</small> ${message}</li>`);
    document.getElementById("group-message-text").value = '';
    scrollToBottom();
});

connection.on("ReceiveMessage", (user, message) => {
    var li = document.createElement("li");
    li.classList.add("list-group-item");

    var formattedTimestamp = new Date().toLocaleString('en-US', { hour12: true });

    li.innerHTML = `<small class="text-muted">${formattedTimestamp}</small> <strong>${user}: </strong>${message}`;
    document.getElementById("messageList").appendChild(li);
    scrollToBottom();
});

document.getElementById("groupmsg").addEventListener("click", async (event) => {
    var groupName = document.getElementById("group-name").value;
    var userName = document.getElementById("user-name").value;
    var groupMsg = document.getElementById("group-message-text").value;

    if (groupMsg === "") { // Eğer mesaj boşsa işlem yapma
        return;
    }

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
        var response = await fetch(`/GroupChat/GetMessagesByGroupId`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ MessageGroupId: groupName })
        });

        if (!response.ok) {
            throw new Error('Mesajlar alınırken hata oluştu.');
        }

        var messages = await response.json();

        document.getElementById('messageList').innerHTML = '';

        messages.forEach(message => {
            var li = document.createElement('li');
            li.classList.add('list-group-item');
            li.innerHTML = `<small class="text-muted">${new Date(message.createdTime).toLocaleString()}</small> <strong>${message.senderUser}:</strong> ${message.message}`;
            document.getElementById('messageList').appendChild(li);
        });

        await connection.invoke("AddToGroup", groupName, userName);
        scrollToBottom();
    } catch (e) {
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

connection.start().then(() => { });

document.addEventListener('DOMContentLoaded', (event) => {
    scrollToBottom();
    document.addEventListener("keypress", function (event) {
        if (event.keyCode === 13) {
            document.getElementById("groupmsg").click();
        }
    });
});

function scrollToBottom() {
    var messagesList = document.getElementById('messageList');
    messagesList.scrollTop = messagesList.scrollHeight;
}
