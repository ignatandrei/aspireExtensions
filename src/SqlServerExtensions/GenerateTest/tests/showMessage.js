function showMessageForVideo(message,seconds) {
    //window.alert('test');
    // Create a div for the message
    const overlayDiv = document.createElement('div');
        overlayDiv.style.position = 'fixed';
    overlayDiv.style.top = '0';
    overlayDiv.style.left = '0';
    overlayDiv.style.width = '100%';
    overlayDiv.style.height = '100%';
    overlayDiv.style.backgroundColor = 'rgba(255, 204, 0, 0.8)'; // Semi-transparent yellow
    overlayDiv.style.color = '#000';
    overlayDiv.style.display = 'flex';
    overlayDiv.style.alignItems = 'center';
    overlayDiv.style.justifyContent = 'center';
    overlayDiv.style.zIndex = '9999';
    var table = document.createElement("table");
    message.split('\n').forEach((line) => {
        let tr = document.createElement('tr');
        let td1 = document.createElement('td');
        line=line.trim();
        var newBlockDiv = document.createElement("div");
         newBlockDiv.innerHTML=line;
         td1.appendChild(newBlockDiv);
         tr.appendChild(td1);
         table.appendChild(tr);
    });
    overlayDiv.appendChild(table);
    //overlayDiv.innerHTML = message;
    // Append the div to the body
    document.body.appendChild(overlayDiv);

    // Set a timeout to remove or hide the message after 10 seconds
    setTimeout(() => {
        overlayDiv.remove(); // Or use messageDiv.style.display = 'none';
    }, seconds*1000);
};
window.showMessageForVideo = showMessageForVideo;