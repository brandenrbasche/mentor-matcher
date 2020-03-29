google.charts.load('current', { 'packages': ['corechart'] });
google.charts.load('current', { 'packages': ['gauge'] });
google.charts.setOnLoadCallback(getChartData());

window.onload = function () {
    chartGaugeCall();
}

/* -----------------------------
    CODE FOR SIGN IN PAGE
 --------------------------------- */
function loginValidation() {
    let user_ID = $("#username").val();
    let Password = $("#password").val();
    let loginArr = [user_ID, Password];

    for (var i = 0; i < loginArr.length; i++) {
        if (loginArr[i] == "") {
            alert("Please complete all fields.");
            break;
        } else {
            location.href = "gradeCalculator.html"
            userLogin(loginArr);
            break;
        }
    }

    location.href=""
}

function userLogin(loginArr) {
    $.ajax({
        url: 'AccountServices.asmx/LogOn',
        type: 'POST',
        data: JSON.stringify(loginArr),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (msg) {
            console.log("Succesfully logged in.");
            location.href = "gradeCalculator.html"
        },
        //error: function () {
        //    alert("Username or password is incorrect.");
        //}
    });
}




/* -----------------------------
    CODE FOR REGISTER PAGE
 --------------------------------- */

function registerOnSubmit() {
    // let user = $("#user").val();
    let uid = $("#uid").val();
    let uni = $("#uni").val();
    let fname = $("#fname").val();
    let lname = $("#lname").val();
    let pass = $("#pass").val();
    let confirm = $("#confirm").val();
    let major = $("#major").val();
    let responses = [uid, uni, fname, lname, pass, confirm, major];

    for (var i = 0; i < responses.length; i++) {
        if (pass != confirm) {
            alert("Passwords don't match.");
            break;
        }

        if (responses[i] == "") {
            alert("Please complete all fields.");
            break;
        } else {
            insertSignUpData(responses);
            alert("Account succesfully created!");
            window.location.href = "index.html"
            break;
        }
    }
}

// Put data into database
function insertSignUpData(responses) {
    let userData = {
        "user_ID": responses[0],
        "University_name": responses[1],
        "Fname": responses[2],
        "Lname": responses[3],
        "Password": responses[4],
        "Major": responses[6],
        "AdminStatus": 0
    };

    console.log(JSON.stringify(userData));

    // Call C# metohd to insert into table
    $.ajax({
        type: 'POST',
        url: "AccountServices.asmx/RequestAccount",
        data: JSON.stringify(userData),
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (msg) {
            console.log("success");
        }
    });

}

/* -----------------------------
    CODE FOR GRADE ENTRY PAGE
 --------------------------------- */

function submitGradeEntryData() {
    let year = $("#year").val();
    let term = $("#term").val();
    let totalCredits = $("#totalCredits").val();
    let currentGpa = $("#currentGpa").val();
    let gradeResponses = [year, term, totalCredits, currentGpa];

    for (var i = 0; i < gradeResponses.length; i++) {
        if (gradeResponses[i] == "") {
            alert("Please complete all fields!");
            break;
        } else if (gradeResponses[3] > 4) {
            alert("GPA cannot be over 4.0.");
            break;
        } else {
            location.href="gradeCalculator.html"
            insertGradeEntryData(gradeResponses);
            break;
        }
    }
}

function insertGradeEntryData(grades) {
    let gradeResponses = {
        "Year": grades[0],
        "Term": grades[1],
        "Total_Credits": grades[2],
        "CurrentGPA": grades[3]
    };

    $.ajax({
        type: 'POST',
        url: "AccountServices.asmx/HandleGradeEntryData",
        data: JSON.stringify(gradeResponses),
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (msg) {
            console.log("success");
        }
    });
}

// GetAccounts js and ajax call
// pull data for gauge chart

function calculateAvg(arr) {
    var sum = 0;
    for (var i = 0; i < arr.length; i++) {
        sum += arr[i];
    }

    return sum / arr.length;
}

function chartGaugeCall() {
    //var visualization = new google.visualization.Gauge(container);

    $.ajax({
        url: "AccountServices.asmx/GetAccounts",
        type: "POST",
        dataType: "json",
        contextType: "application/json; charset=utf-8",
        data: { action: "GetAccounts" },
        traditional: true,
        success: function (grades) {
            var gpaArrays = [];
            var gpaArr = [];
            var sum = 0;
            var avg;

            for (var key in grades) {
                gpaArrays.push([grades[key].CurrentGPA]);
            }

            gpaArr = gpaArrays.flat();
            var avg = calculateAvg(gpaArr);
            console.log(avg);

            var data = new google.visualization.DataTable();
            data.addColumn('number', "Cumulative GPA");
            data.addRows(1);
            data.setCell(0, 0, avg);

            //for (var i = 0; i < gpaArr.length; i++) {
            //    data.addRows(gpaArr[i]);
            //}

            // add rows and data to DataTable

            var options = {
                //width: 400, height: 120,
                minorTicks: 5,
                min: 0, max: 4,
                greenFrom: 3.5, greenTo: 4
            }

            var gauge = new google.visualization.Gauge(document.getElementById('gaugechart'));
            gauge.draw(data, options);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('Could not get posts, server response: ' + textStatus + ': ' + errorThrown);
        }
    }).responseJSON;
}

function getChartData() {
    $.ajax({
        url: "AccountServices.asmx/GetAccounts",
        type: "POST",
        dataType: "json",
        contextType: "application/json; charset=utf-8",
        data: { action: "GetAccounts" },
        traditional: true,
        success: function (grades) {

            // add rows and data to DataTable
            var data = new google.visualization.DataTable();
            data.addColumn('number', 'Year');
            data.addColumn('number', 'Current GPA');

            for (var key in grades) {
                data.addRows([
                    [grades[key].Year, grades[key].CurrentGPA]
                ]);         
            }

            var options = {
                title: "GPA by Year",
                curveType: "function",
                legend: { position: 'bottom' }
            };

            var chart = new google.visualization.ColumnChart(document.getElementById('linechart'));
            chart.draw(data, options);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('Could not get posts, server response: ' + textStatus + ': ' + errorThrown);
        }
    }).responseJSON;
}




/* -----------------------------
    CODE FOR ADMIN LOGIN PAGE
 --------------------------------- */
function AdminFunction() {
    //console.log("hello");
    $.ajax({
        url: "AccountServices.asmx/GetAccountData",
        type: 'POST',
        dataType: 'json',
        contextType: "application/json; charset=utf-8",
        success: function (res) {
            // console.log(res);
           // console.log(res);
            console.log("hello");
            //console.log(entry);
            i = 0;
            for (var key in res) {
                //console.log(key)
                console.log([res[key].userId, res[key].university, res[key].first_Name, res[key].last_Name, res[key].password, res[key].major]);
                //row2 = [res[key].userId, res[key].university, res[key].first_Name, res[key].last_Name, res[key].password, res[key].major]
                //document.getElementById("row2").innerHTML = row2
                document.getElementById("id" + key).innerHTML = [res[key].user_ID]
                document.getElementById("univ" + key).innerHTML = [res[key].University_name]
                document.getElementById("fname" + key).innerHTML = [res[key].Fname]
                document.getElementById("lname" + key).innerHTML = [res[key].Lname]
                document.getElementById("pass" + key).innerHTML = [res[key].Password]
                document.getElementById("major" + key).innerHTML = [res[key].Major]

                i++
            }

        }
    }).responseJSON;
}
function AdminVal() {
    let id = document.getElementById("username").value;
    let pass = document.getElementById("password").value;
    if (id == 1211111223 && pass == 'Brook_John90!') {
        console.log("true");
    }
    else {
        console.log("false");
        alert("You are wrong");
        continue;
        //document.GetElementById("login")
        return false;
    }
}

/* -----------------------------
    ADMIN FUNCTIONALITY: UPDATE/DELETE ACCOUNTS
 --------------------------------- */

function DeleteAccount() {
    let user_ID = document.getElementById("user_ID").value;
    console.log(user_ID);
    $.ajax({
        url: "AccountServices.asmx/DeleteAccount",
        data: { user_ID: user_ID },
        type: 'POST',
        dataType: 'json',
        contextType: "application/json; charset=utf-8",
        success: function (msg) {
            console.log("success");
            console.log(id + 'nice dude');
        },
        failure: function (msg) {
            console.log("why do you suck");
        }
    })
    $("#user_ID").val("");
}

function UpdateAccount() {
    let user_ID = document.getElementById("user_ID").value;
    let University_name = document.getElementById("University_name").value;
    let Fname = document.getElementById("Fname").value;
    let Lname = document.getElementById("Lname").value;
    let Password = document.getElementById("Password").value;
    let Major = document.getElementById("Major").value;
    let data = [user_ID, University_name, Fname, Lname, Password, Major];
    $.ajax({
        url: "AccountServices.asmx/UpdateUserAccount",
        data: { user_ID, University_name, Fname, Lname, Password, Major },
        type: 'POST',
        dataType: 'json',
        contextType: "application/json; charset=utf-8",
        success: function (msg) {
            AdminFunction();
            console.log("success");
            console.log(data + 'nice dude');
        },
        failure: function (msg) {
            console.log("why do you suck");
        }
    });

    $("#user_ID").val("");
    $("#University_name").val("");
    $("#Fname").val("");
    $("#Lname").val("");
    $("#Password").val("");
}