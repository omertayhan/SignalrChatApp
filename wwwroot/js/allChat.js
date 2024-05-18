"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    li.classList.add("list-group-item"); // Class ekle

    // Tarih ve saat bilgisi ekle
    var currentDate = new Date();
    var timestamp = new Date(currentDate).toISOString(); // İlgili ofset kadar tarih ve saat bilgisi eklenir

    // Formatlanmış tarih ve saat bilgisini elde etmek için
    var formattedTimestamp = new Date(timestamp).toLocaleString('en-US', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: true
    });

    li.innerHTML = `<small class="text-muted">${formattedTimestamp}</small> <strong>${user}: </strong>${message}`;
    document.getElementById("dbmessagesList").appendChild(li);
    scrollToBottom();
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;

    if (message === "") { // Eğer mesaj boşsa işlem yapma
        return;
    }

    var fullMessage = ` ${message}`;

    connection.invoke("SendMessage", user, fullMessage).catch(function (err) {
        return console.error(err.toString());
    });

    // Tarih ve saat bilgisi ekle
    var currentDate = new Date();
    var offset = 3 * 60; // GMT+3 için dakika cinsinden ofset hesaplanır (3 saat * 60 dakika)
    var dbtimestamp = new Date(currentDate.getTime() + offset * 60000).toISOString(); // İlgili ofset kadar tarih ve saat bilgisi eklenir

    // Mesajı veritabanına kaydet
    fetch("/Home/SaveMessage", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            SenderUser: user,
            message: message,
            CreatedTime: dbtimestamp
        })
    }).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("messageInput").value = '';
    event.preventDefault();
});

document.addEventListener('DOMContentLoaded', (event) => {
    scrollToBottom();
    document.addEventListener("keypress", function (event) {
        // Enter tuşunun keyCode değeri 13'tür
        if (event.keyCode === 13) {
            document.getElementById("sendButton").click();
        }
    });
});

function scrollToBottom() {
    var messagesList = document.getElementById('dbmessagesList');
    messagesList.scrollTop = messagesList.scrollHeight;
}
