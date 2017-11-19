// App's JavaScript
'use strict';

var lastmessage = undefined;
// create random user id
var epoch = (new Date).getTime();
var source = new EventSource(
    '/event-stream?channel=hash&t=' + (new Date).getTime());

var dataProcessor =
{
        processMessage: function(msg) {
            console.log(lastmessage == msg);
            if (msg.subscriber !== null && msg.subscriber !== undefined) {
                // And if the subscriber == self, then show the hash
                
                if (msg.subscriber == epoch) {
                    if (msg.succeed) {
                        var text = 'The page "' + msg.url + '" has a MD5 hash "' + msg.result + '".';
                        $('#status').html(text);
                    } else {
                        var text = 'The page "' + msg.url + '" cannot be processed. Server returned error "' + msg.result + '".';
                        $('#status').html(text);
                    }
                }
            }
        },
        submit(url) {
            // Automatically add 'http' to the url if not presented, but user may specify http or https if 
            // they wish
            if (!url.startsWith('http://'.toLowerCase()) && !url.startsWith('https://'.toLowerCase()))
                url = 'http://' + url;

            $('#status').html('Processing your request...');
            var endpoint = '/api/md5?url=' + url + '&subscriber=' + epoch;
            $.ajax({
                method: "GET",
                url: endpoint,
                cache: false,
                success: function (result) {

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $('#status').html('Server returned error: ' + errorThrown);
                }
            });
        }
}

source.addEventListener('error', function (e) {
    $('#status').html('Unable to subscribe server-side event.');
}, false);

$(source).handleServerEvents({
    handlers: {
        // The only thing we have to process is the message
        onMessage: function (msg, e) {
            console.log(msg);
            lastmessage = msg;
            // Find corrsponding messages from broadcast
            setTimeout(dataProcessor.processMessage(lastmessage), 100);
            // $.timeout(100, dataProcessor);
            // dataProcessor.processMessage(msg);
        }
    }
});

$('#hash-button').click(function () {
    var url = $('#url-input').val();
    if (url && url.trim()) {
        dataProcessor.submit(url);
        dataProcessor.submit(url);
    } else {
        $('#status').html('Please fill in the field before submit.');
    }
});

$('#url-input').keypress(function (key) {
    if (key.keyCode == 13)
        $('#hash-button').click();
});