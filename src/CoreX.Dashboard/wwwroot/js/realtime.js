"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/devdash/realtime").build();

connection.on("RequestStarted", function (request) {
    
    var tbody = document.getElementById('requests').getElementsByTagName('tbody')[0];
    var row = tbody.insertRow(0);
    row.id = request.id;
    row.insertCell().innerHTML = '<div><i class="fas fa-circle"></i></div>';
    row.insertCell().innerHTML = '<div></div>';
    row.insertCell().innerHTML = '<div>' + request.method + '</div>';
    row.insertCell().innerHTML = '<div>' + request.path + '</div>';
    row.insertCell().innerHTML = '<div></div>';

    var badges = document.getElementsByClassName('requests-badge');
    for (var i = 0; i < badges.length; i++) {
        var count = parseInt(badges[i].innerHTML);
        count++;
        badges[i].innerHTML = count.toString();
    }

    setTimeout(function () {
        row.className = "in";
    }, 0);
});

connection.on("RequestEnded", function (request) {

    var row = document.getElementById(request.id);
    if (row) {
        var cells = row.getElementsByTagName('td');
        if (cells.length > 4) {
            cells[0].className = getColor(request.status);
            cells[1].className = getColor(request.status);
            cells[1].innerHTML = '<div>' + request.status + '</div>';
            cells[4].innerHTML = '<div class="number">' + request.duration.toLocaleString() + ' ms</div>';
        }
    }
});

connection.on("ExceptionAdded", function (ex) {

    var tbody = document.getElementById('exceptions').getElementsByTagName('tbody')[0];
    var row = tbody.insertRow(0);
    
    row.insertCell().innerHTML = '<div>' + ex.path + '</div>';
    row.insertCell().innerHTML = '<div>' + ex.type + '</div>';
    row.insertCell().innerHTML = '<div>' + ex.message + '</div>';
    row.insertCell().innerHTML = '<div class="code">' + ex.stackTrace + '</div>';

    var badges = document.getElementsByClassName('errors-badge');
    for (var i = 0; i < badges.length; i++) {
        var count = parseInt(badges[i].innerHTML);
        count++;
        badges[i].innerHTML = count.toString();
    }

    setTimeout(function () {
        row.className = "in";
    }, 0);
});

function getColor(status) {
    if (status.length === 0)
        return '';

    if (status[0] === '2')
        return 'text-success';

    if (status[0] === '3')
        return 'text-success';

    if (status[0] === '4')
        return 'text-warning';

    if (status[0] === '5')
        return 'text-danger';

    return '';
}

connection.start().then(function () {
    console.info('Realtime connection started');
}).catch(function (err) {
    return console.error(err.toString());
});