"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    li.classList.add("list-group-item"); // Class ekle
    document.getElementById("messagesList").appendChild(li);
    li.textContent = `${user} : ${message}`;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;

    // Tarih ve saat bilgisi ekle
    var currentDate = new Date();
    var timestamp = currentDate.toISOString(); // ISO formatında string

    var fullMessage = `${timestamp} - ${message}`;

    connection.invoke("SendMessage", user, fullMessage).catch(function (err) {
        return console.error(err.toString());
    });

    // Mesajı veritabanına kaydet
    fetch("/Home/SaveMessage", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            SenderUser: user,
            message: message,
            CreatedTime: timestamp
        })
    }).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});
