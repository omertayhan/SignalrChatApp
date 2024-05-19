"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/privateChatHub").build();

// Disable send button until connection is established
document.getElementById("sendToUser").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    li.classList.add("list-group-item");

    var formattedTimestamp = new Date().toLocaleString('en-US', { hour12: true });

    li.innerHTML = `<small class="text-muted">${formattedTimestamp}</small> <strong>${user}: </strong>${message}`;
    document.getElementById("messagesList").appendChild(li);
    document.getElementById("messageInput").value = '';
});

connection.start().then(function () {
    connection.invoke("GetConnectionId").then(function (id) {
        document.getElementById("connectionId").value = id;
    });
    document.getElementById("sendToUser").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendToUser").addEventListener("click", async function (event) {
    var user = document.getElementById("userInput").value;
    var receiverUserName = document.getElementById("receiverId").value;
    var message = document.getElementById("messageInput").value;

    try {
        var response = await fetch(`/PrivateChat/GetMessages`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ SenderUser: user, ReceiverUser: receiverUserName })
        });

        var messages = await response.json();

        // Eğer messages tanımlanmamışsa ya da boşsa, işlemi geç
        if (!messages || messages.length === 0) {
            console.log("Mesaj bulunamadı.");
            return;
        }

        document.getElementById('messagesList').innerHTML = '';

        messages.forEach(message => {
            var li = document.createElement('li');
            li.classList.add('list-group-item');
            li.innerHTML = `<small class="text-muted">${new Date(message.createdTime).toLocaleString()}</small> <strong>${message.senderUser}:</strong> ${message.message}`;
            document.getElementById('messagesList').appendChild(li);
        });

        await connection.invoke("SendToUser", user, receiverUserName, message);
        scrollToBottom();
    } catch (error) {
        console.error(error.toString());
    }

    event.preventDefault();
});

document.addEventListener('DOMContentLoaded', (event) => {
    document.addEventListener("keypress", function (event) {
        if (event.keyCode === 13) {
            document.getElementById("sendToUser").click();
        }
    });
});

function scrollToBottom() {
    var messagesList = document.getElementById('messagesList');
    messagesList.scrollTop = messagesList.scrollHeight;
}