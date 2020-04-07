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
    if (responses[4].value != responses[5].value) {
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
    // var menteeResponses = [responses[0].value, responses[1].value, responses[2].value, responses[3].value, responses[4].value, 0];
    var menteeResponses = {
        "userName": responses[0].value,
        "fName": responses[1].value,
        "lName": responses[2].value,
        "email": responses[3].value,
        "password": responses[4].value,
        "userType": 0
    };

    var matchingObj = {
        "userName": responses[0].value,
        "q1": responses[6],
        "q2": responses[7],
        "q3": responses[8],
        "q4": responses[9]
    };

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

    // Insert 
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
                                        document.getElementById("welcomeNameId").innerHTML = firstName;
                                        console.log(mentStatus);
                                    }
                                    else {
                                        window.location.href = "MenteeProfile.html"
                                        document.getElementById("welcomeId").innerHTML = firstName;
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
    MENTEE / MENTOR PROFILE
--------------------------------- */
function passUserName() {
    console.log(localStorage.getItem('userName'));
    let welcomeName = document.getElementById('welcomeNameId');
    let userLogin = localStorage.getItem('userName');
    welcomeName.innerHTML = userLogin;
}



// Create a "close" button and append it to each list item
var myNodelist = document.getElementsByTagName("LI");
var i;
for (i = 0; i < myNodelist.length; i++) {
  var span = document.createElement("SPAN");
  var txt = document.createTextNode("\u00D7");
  span.className = "close";
  span.appendChild(txt);
  myNodelist[i].appendChild(span);
}

// Click on a close button to hide the current list item
var close = document.getElementsByClassName("close");
var i;
for (i = 0; i < close.length; i++) {
  close[i].onclick = function() {
    var div = this.parentElement;
    div.style.display = "none";
  }
}

// Add a "checked" symbol when clicking on a list item
var list = document.querySelector('ul');
list.addEventListener('click', function(ev) {
  if (ev.target.tagName === 'LI') {
    ev.target.classList.toggle('checked');
  }
}, false);

// Create a new list item when clicking on the "Add" button
function newElement() {
  var li = document.createElement("li");
  var inputValue = document.getElementById("myInput").value;
  var t = document.createTextNode(inputValue);
  li.appendChild(t);
  if (inputValue === '') {
    alert("You must write something!");
  } else {
    document.getElementById("myUL").appendChild(li);
  }
  document.getElementById("myInput").value = "";

  var span = document.createElement("SPAN");
  var txt = document.createTextNode("\u00D7");
  span.className = "close";
  span.appendChild(txt);
  li.appendChild(span);

  for (i = 0; i < close.length; i++) {
    close[i].onclick = function() {
      var div = this.parentElement;
      div.style.display = "none";
    }
  }
}