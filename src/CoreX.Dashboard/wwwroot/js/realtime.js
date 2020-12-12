"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/devdash/realtime").build();

connection.on("RequestStarted", function (request) {
    console.info('Request Started: ' + request.path);

    var tbody = document.getElementById('requests').getElementsByTagName('tbody')[0];
    var row = tbody.insertRow(0);
    row.id = request.id;
    row.insertCell().innerHTML = '<div><i class="fas fa-circle"></i></div>';
    row.insertCell().innerHTML = '<div></div>';
    row.insertCell().innerHTML = '<div>' + request.method + '</div>';
    row.insertCell().innerHTML = '<div>' + request.path + '</div>';
    row.insertCell().innerHTML = '<div></div>';

    setTimeout(function () {
        row.className = "in";
    }, 0);
});

connection.on("RequestEnded", function (request) {
    console.info('Request Ended: ' + request.path);

    var row = document.getElementById(request.id);
    if (row) {
        var cells = row.getElementsByTagName('td');
        if (cells.length > 4) {
            cells[0].className = getColor(request.status);
            cells[1].className = getColor(request.status);
            cells[1].innerHTML = '<div>' + request.status + '</div>';
            cells[4].innerHTML = '<div>' + request.duration + ' ms</div>';
        }
    }
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