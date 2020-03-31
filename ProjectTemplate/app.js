/* -----------------------------
    REGISTER FORM VALIDATION
--------------------------------- */
function formValidation(form) {
    var responses = form.elements;
    if(!form.checkValidity()) {
        for(var i=0; i<responses.length; i++) {
            if(responses[i].value == "") {
                alert("Cannot leave fields blank.");
                break;
            }

            if(!form[3].checkValidity()) {
                alert("Please enter a correct email.");
                break;
            }
        }
    }
    if(responses[4].value != responses[5].value) {
        alert("Passwords must match.");
    }

    if(window.location.href.includes("mentee_register.html")) {
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
        "q1": responses[6].value,
        "q2": responses[7].value,
        "q3": responses[8].value,
        "q4": responses[9].value
    };

    // Inserts menteeResponses into the user_table table!
    $.ajax({
        type: 'POST',
        url:"../AccountServices.asmx/RegisterAccount",
        data: JSON.stringify(menteeResponses),
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function(msg) {
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
            window.location.href = "index.html"
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