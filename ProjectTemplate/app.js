/* -----------------------------
    REGISTER FORM VALIDATION
--------------------------------- */
function formValidation(form) {
    var responses = form.elements;
    console.log(responses);
    if (!form.checkValidity()) {
        for (var i = 0; i < responses.length; i++) {
            if (responses[i].value == "") {
                alert("Cannot leave fields blank.");
                break;
            }
            
            if (!form[3].checkValidity()) {
                alert("Please enter a correct email.");
                break;
            }
        }
    }
    else if (responses[4].value != responses[5].value) {
        alert("Passwords must match.");
    }

    if (window.location.href.includes("mentee_register.html")) {
        registerMentee(responses);
    } else {
        registerMentor(responses);
    }

}

/* -----------------------------
    MENTEE REGISTRATION
--------------------------------- */
function registerMentee(responses) {
    let userName = responses[0].value;
    var menteeResponses = {
        "userName": responses[0].value,
        "fName": responses[1].value,
        "lName": responses[2].value,
        "email": responses[3].value,
        "password": responses[4].value,
        "userType": 0
    };
    console.log(menteeResponses);

    var matchingObj = {
        "userName": responses[0].value,
        "q1": responses[6].value,
        "q2": responses[7].value,
        "q3": responses[8].value,
        "q4": responses[9].value
    };
    console.log(matchingObj);

    for (var i = 0; i < 4; i++) {
        let responseObj = {
            "userName": responses[0].value,
            "responseId": responses[i+6].value
        }
        // Inserts responseObj into user_responses_table!
        $.ajax({
            type: 'POST',
            url: '../AccountServices.asmx/InsertResponseValues',
            data: JSON.stringify(responseObj),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (msg) {
                console.log("Response " + i + " added to user_responses_table!");
            }
        });

    };

    // Inserts menteeResponses into the user_table table!
    $.ajax({
        type: 'POST',
        url: "../AccountServices.asmx/RegisterAccount",
        data: JSON.stringify(menteeResponses),
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (msg) {
            console.log("menteeResponses succesfully added to user_table");
        }
    });

    // Inserts matchingObj into the responses table!
    $.ajax({
        type: 'POST',
        url: "../AccountServices.asmx/InsertMatchingResponses",
        data: JSON.stringify(matchingObj),
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (msg) {
            console.log("matchingObj sucessfully added to responses table");
            alert("Mentee account created!");
            //window.location.href = "index.html"
        }
    });

    // MATCHING ELAGANZA EXTRAVAGANZA!
    // when the user registers, their username is processed to call GetMatches, then they are redirected to match.html
    $.ajax({
        type: 'POST',
        url: "../AccountServices.asmx/GetMatches",
        data: JSON.stringify({ "userName": userName }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'text',
        success: function (data) {
            console.log(data);
            localStorage.setItem('matches', data);
            console.log("GetMatches method called!");
            window.location = 'match.html';
        }
    });


}

/* -----------------------------
    MENTOR REGISTRATION
--------------------------------- */
function registerMentor(responses) {
    var mentorResponses = {
        "userName": responses[0].value,
        "fName": responses[1].value,
        "lName": responses[2].value,
        "email": responses[3].value,
        "password": responses[4].value,
        "userType": 1
    };

    var matchingObj = {
        "userName": responses[0].value,
        "q1": responses[6].value,
        "q2": responses[7].value,
        "q3": responses[8].value,
        "q4": responses[9].value
    };

    for (var i = 0; i < 4; i++) {
        let responseObj = {
            "userName": responses[0].value,
            "responseId": responses[i + 6].value
        }
        // Inserts responseObj into user_responses_table!
        $.ajax({
            type: 'POST',
            url: '../AccountServices.asmx/InsertResponseValues',
            data: JSON.stringify(responseObj),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (msg) {
                console.log("Response " + i + " added to user_responses_table!");
            }
        });

    };

    // Inserts mentorResponses into the user_table table!
    $.ajax({
        type: 'POST',
        url: "../AccountServices.asmx/RegisterAccount",
        data: JSON.stringify(mentorResponses),
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (msg) {
            console.log("mentorResponses succesfully added to user_table");
        }
    });

    // Inserts matchingObj into the responses table!
    $.ajax({
        type: 'POST',
        url: "../AccountServices.asmx/InsertMatchingResponses",
        data: JSON.stringify(matchingObj),
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (msg) {
            console.log("matchingObj sucessfully added to responses table");
            alert("Mentor account created!");
            window.location.href = "index.html"
        }
    });
}

/* -----------------------------
    MENTEE/MENTOR LOGIN
--------------------------------- */
function logOn() {
    let userName = document.getElementById("userName").value;
    let Password = document.getElementById("password").value;
    console.log(userName);
    console.log(Password);
    var userEntry = '';
    var firstName = '';
    loginInfo = {
        "userName": userName,
        "Password": Password
    };
    getData = { "userName": userName };

    localStorage.setItem('userName', userName);

    $.ajax({
        type: 'POST',
        url: "../AccountServices.asmx/LogOn",
        data: JSON.stringify(loginInfo),
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (msg) {
            console.log(msg);
            for (key in msg) {
                console.log(key, msg[key]);
                console.log(msg[key]);
                trueFalse = msg[key];
            }
                
            if (trueFalse == true) {
                //alert("Login Successful");
                $.ajax({
                    type: 'POST',
                    url: "../AccountServices.asmx/GetAccountData",
                    data: JSON.stringify(getData),
                    contentType: "application/json; charset=utf-8",
                    dataType: 'json',
                    success: function (data) {
                        console.log(data);
                        for (key in data) {
                            console.log(key, data[key]);
                            console.log(data[key][0]);
                            var newData = data[key][0];
                            for (yam in newData) {
                                console.log(yam);
                                var temp = yam;
                                if (temp == 'fName') {
                                    firstName = newData[yam];
                                    console.log(firstName);
                                    console.log(newData[yam]);

                                }
                                if (temp == 'userType') {

                                    var mentStatus = newData[yam];
                                    console.log(mentStatus);
                                    if (mentStatus == 1) {
                                        window.location.href = "MentorProfile.html"
                                        //document.getElementById("welcomeNameId").innerHTML = firstName;
                                        console.log(mentStatus);
                                    }
                                    else {
                                        window.location.href = "MenteeProfile.html"
                                        //document.getElementById("welcomeId").innerHTML = firstName;
                                    }
                                }
                                console.log(yam[1]);
                            }
                            console.log(yam + ' ' + newData[yam]);
                            if (yam = userName) {
                                console.log(yam);
                                userEntry = yam;
                            }
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("Wrong username or password. Try again!");
                        console.log(XMLHttpRequest);
                        console.log(textStatus);
                        console.log(errorThrown);
                    }
                });
            }
                
                else {
                    alert("You have failed the vibe check");
                }      
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("you failure");
        }
    });

}
/* -----------------------------
    ANONYMOUS EMAIL
--------------------------------- */
function anonEmail() {
    let userName = localStorage.getItem('userName');
    console.log(localStorage.getItem('userName'));
    
    getData = { "userName": userName };
    $.ajax({
        type: 'POST',
        url: "../AccountServices.asmx/GetMatches",
        data: JSON.stringify(getData),
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (res) {
            console.log(res);
            window.location.href = "emailPage.html"
           
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
}
function getAnon() {
    let subject = document.getElementById("subject").value;
    let body = document.getElementById("body").value;
    let recipient = "jacobnorwood10@gmail.com";
    console.log("You got here bro");
    emailInfo = {

        "subject": subject,
        "body": body,
        "recipient": recipient
    }
    $.ajax({
        type: 'POST',
        url: "../AccountServices.asmx/AnonEmail",
        data: JSON.stringify(emailInfo),
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (res) {
            console.log(res);
            console.log("Hey nice bro");
        }
    });
}
/* -----------------------------
    MENTEE / MENTOR PROFILE
--------------------------------- */
function passUserName() {
    let welcomeName = document.getElementById('welcomeNameId');
    let userLogin = localStorage.getItem('userName');
    welcomeName.innerHTML = userLogin;
}

var myNodeList = document.getElementsByTagName('li');
for (var i = 0; i < myNodeList.length; i++) {
    var span = document.createElement('span');
    var text = document.createTextNode('\u00D7');
    span.className = "close";
    span.appendChild(text);
    myNodeList[i].appendChild(span);
}
// Hide list item when close button is pressed
var close = document.getElementsByClassName('close');
for (var i = 0; i < close.length; i++) {
    close[i].onclick = function () {
        var div = this.parentElement;
        div.style.display = "none";
        console.log(div);
    }
}

var checkStatus = 1;
var list = document.querySelector('ul');
list.addEventListener('click', function (ev) {
    if (ev.target.tagName === 'LI') {
        checkStatus = 0;
        ev.target.classList.toggle('checked');
    }
}, false);
// Create a new list item when clicking the "add" button
function newElement() {
    var li = document.createElement('li');
    var inputValue = document.getElementById('goalInput').value;
    var t = document.createTextNode(inputValue);
    console.log(inputValue);
    li.appendChild(t);
    if (inputValue === '') {
        alert("You must write something!");
    } else {
        document.getElementById('goalUl').appendChild(li);
    }
    document.getElementById('goalInput').value = "";
    var span = document.createElement('span');
    var txt = document.createTextNode("\u00D7");
    span.className = "close";
    span.appendChild(txt);
    li.appendChild(span);
    for (var i = 0; i < close.length; i++) {
        close[i].onclick = function () {
            var div = this.parentElement;
            div.style.display = "none";
        }
    }
    let userName = localStorage.getItem('userName');
    getData = {
        "userName": userName,
        "myGoal": inputValue,
        "goalStatus": checkStatus
    };
    $.ajax({
        type: 'POST',
        url: "../AccountServices.asmx/InsertGoals",
        data: JSON.stringify(getData),
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (res) {
            console.log(res);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
}

// Add a "checked" symbol when clicking on a list item
var list = document.querySelector('ul');
list.addEventListener('click', function (ev) {
  if (ev.target.tagName === 'LI') {
    ev.target.classList.toggle('checked');
  }
}, false);

/* -----------------------------
    PICK YOUR MATCH!
--------------------------------- */
function getMatchData(arr) {
    console.log(arr);
    let welcomeName = document.getElementById('welcomeNameId');
    welcomeName.innerHTML = arr[0].mentee;
    let ul = document.querySelector('ul');
    var matchNumber = 0;
    for (var i = 0; i < arr.length; i++) {
        matchNumber += 1;
        var mentorName = arr[i].mentor;
        var mentorEmail = arr[i].mentorEmail;
        var commonality = arr[i].commonality;
        createList(matchNumber, mentorName, commonality);
    }

}

// create account array here
var myArr = [];

function createList(matchNumber, matchName, commonality) {
    let matchString = "Match " + matchNumber + ": " + matchName;
    let h4 = document.createElement('h4');
    h4.innerHTML = matchString;

    let p = document.createElement('p');
    p.innerHTML = "Commonality: " + calcCommonality(commonality);

    let btn = document.createElement('button');
    let btnId = matchName;
    btn.innerHTML = "Select This Mentor";
    btn.setAttribute("id", matchName);
    btn.setAttribute("onclick", "selectMentor(this.id)");

    let hr = document.createElement('hr');

    document.body.appendChild(h4);
    document.body.appendChild(p);
    document.body.appendChild(btn);
    document.body.appendChild(hr);
}

function calcCommonality(commonality) {
    return ((commonality / 4) * 100) + "%";
}

function selectMentor(btnId) {
    console.log(btnId);
    let match = JSON.parse(localStorage.getItem("cleanMatches"));
    for (var i = 0; i < match.length; i++) {
        switch (btnId.toString()) {
            case match[i].mentor.toString():
                console.log('switch working');
                localStorage.setItem('matchInfo', match[i]);
                matchMentee(match[i].mentee, match[i].mentor);
                break;
        }
    }
}

function matchMentee(userName, match) {
    console.log('matchMentee function ran!');
    let obj = {
        "userName": userName,
        "match": match
    };
    $.ajax({
        type: 'POST',
        url: "../AccountServices.asmx/UpdateMenteeMatch",
        data: JSON.stringify(obj),
        contentType: 'application/json; charset=utf-8;',
        dataType: 'json',
        success: function () {
            console.log("Succesfully called UpdateMenteeMatch method");
            matchMentor(match, userName);
        },
        error: function () {
            console.log("UpdateMenteeMatch method not called");
        }
    });
}

function matchMentor(match, userName) {
    console.log('matchMentor function ran!');
    let obj = {
        "match": match,
        "userName": userName
    };

    $.ajax({
        type: 'POST',
        url: "../AccountServices.asmx/UpdateMentorMatch",
        data: JSON.stringify(obj),
        contentType: 'application/json; charset=utf-8;',
        dataType: 'json',
        success: function () {
            console.log("Sucessfully called UpdateMentorMatch method");
            matchMentor(match, userName);
            window.location = "MenteeProfile.html";
        },
        error: function () {
            console.log("UpdateMentorMatch method not called");
        }
    });
}

// POPULATE PROFILE PAGE DATA
function getGoals(userName) {
    $.ajax({
        type: 'POST',
        url: '../AccountServices.asmx/GetGoalData',
        data: { "userName": userName },
        dataType: 'text',
        success: function (data) {
            var json = convertXml(data);
            var goalsJson = json.ArrayOfGoals.Goals;
            for (var i = 0; i < goalsJson.length; i++) {
                populateGoalUl(goalsJson[i].myGoal);
            }
        }
    });
}

function populateGoalUl(goal) {
    let goalUl = document.getElementById('goalUl');
    let li = document.createElement('li');
    li.innerHTML = goal;
    goalUl.appendChild(li);
    var span = document.createElement('span');
    var txt = document.createTextNode("\u00D7");
    span.className = "close";
    span.appendChild(txt);
    li.appendChild(span);
}

function initGoals() {
    var close = document.getElementsByClassName('close');
    for (var i = 0; i < close.length; i++) {
        close[i].onclick = function () {
            var div = this.parentElement;
            div.style.display = "none";
        }
    }
}

//var close = document.getElementsByClassName('close');
//var close = document.querySelectorAll('.close');
//console.log(close);
//for (var i = 0; i < close.length; i++) {
//    close[i].onclick = function () {
//        console.log('closed');
//        var div = this.parentElement;
//        div.style.display = "none";
//    }
//}

function deleteGoal() {
    console.log('deleteGoal function ran');

}

function convertXml(xml) {
    var x2js = new X2JS();
    var jsonObj = x2js.xml_str2json(xml);
    return jsonObj;
}

function populateProfile(userName) {
    $.ajax({
        type: 'POST',
        url: "../AccountServices.asmx/GetAccountData",
        data: { "userName": userName },
        dataType: 'text',
        success: function (data) {
            console.log(data);
            var x2js = new X2JS();
            var jsonObj = x2js.xml_str2json(data);
            console.log(jsonObj);
            var userInfo = jsonObj.ArrayOfAccount.Account;
            populateWelcomeName(userInfo.userName);
            loadMatchAccountData(userInfo.match);
        }
    });
}

function loadMatchAccountData(userName) {
    $.ajax({
        type: 'POST',
        url: '../AccountServices.asmx/GetAccountData',
        data: { "userName": userName },
        dataType: 'text',
        success: function (data) {
            var x2js = new X2JS();
            var jsonObj = x2js.xml_str2json(data);
            console.log(jsonObj);
            var userInfo = jsonObj.ArrayOfAccount.Account;
            populateMatchUsername(userInfo.fName, userInfo.lName, userInfo.userName, userInfo.email);
        }
    });
}

function populateWelcomeName(name) {
    let welcomeName = document.getElementById('welcomeNameId');
    welcomeName.innerHTML = name;
}

function populateMatchUsername(firstName, lastName, username, email) { 
    let matchName = document.getElementById('matchNameId');
    let matchUsername = document.getElementById('matchUsernameId');
    let matchEmail = document.getElementById('matchEmailId');

    let matchString = firstName + " " + lastName;
    let emailString = "mailto:" + email;

    matchName.innerHTML = matchString;
    matchUsername.innerHTML = username;
    matchEmail.setAttribute('href', emailString);
}

//function parseXml(data) {
//    let parsed = $.parseXml(data);
//    return parsed;
//}

// Used to toggle the menu on smaller screens when clicking on the menu button
function openNav() {
    var x = document.getElementById("navDemo");
    if (x.className.indexOf("w3-show") == -1) {
      x.className += " w3-show";
    } else { 
      x.className = x.className.replace(" w3-show", "");
    }
}

/* -----------------------------
    AJAX CALLS FOR GOAL TRACKING
--------------------------------- */
