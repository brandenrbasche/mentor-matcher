//and we need this to manipulate data from a db
using System.Data;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
//we need these to talk to mysql
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;

namespace accountmanager
{
	/// <summary>
	/// Summary description for AccountServices
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	[System.Web.Script.Services.ScriptService]
	public class AccountServices : System.Web.Services.WebService
	{

		[WebMethod(true)]
		public bool LogOn(string userName, string Password)
		{
			//LOGIC: pass the parameters into the database to see if an account
			//with these credentials exist.  If it does, then return true.  If
			//it doesn't, then return false
			//we return this flag to tell them if they logged in or not
			bool success = false;
			//our connection string comes from our web.config file like we talked about earlier
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			//here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
			string sqlSelect = "SELECT userName FROM user_table WHERE userName=@idValue and Password=@passValue";
			//set up our connection object to be ready to use our connection string
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			//set up our command object to use our connection, and our query
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
			//tell our command to replace the @parameters with real values
			//we decode them because they came to us via the web so they were encoded
			//for transmission (funky characters escaped, mostly)
			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(userName));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(Password));
			//a data adapter acts like a bridge between our command object and 
			//the data we are trying to get back and put in a table object
			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			//here's the table we want to fill with the results from our query
			DataTable sqlDt = new DataTable();
			//here we go filling it!
			sqlDa.Fill(sqlDt);
			//check to see if any rows were returned.  If they were, it means it's 
			//a legit account
			if (sqlDt.Rows.Count > 0)
			{
				//flip our flag to true so we return a value that lets them know they're logged in
				success = true;
                
			}
			//return the result!
			return success;
		}

		[WebMethod(EnableSession = true)]
		public bool LogOff()
		{
			//if they log off, then we remove the session.  That way, if they access
			//again later they have to log back on in order for their ID to be back
			//in the session!
			Session.Abandon();
			return true;
		}

		//EXAMPLE OF AN INSERT QUERY WITH PARAMS PASSED IN.  BONUS GETTING THE INSERTED ID FROM THE DB!
		[WebMethod(true)]
		public void RegisterAccount(string userName, string fName, string lName, string email, string password, int userType)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			//the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
			//does is tell mySql server to return the primary key of the last inserted row.
			string sqlSelect = "INSERT INTO user_table (userName, fName, lName, email, password, userType) " +
				"VALUES (@userNameValue, @fNameValue, @lNameValue, @emailValue, @passwordValue, @userTypeValue); ";
			//"SELECT LAST_INSERT_ID();";
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@userNameValue", userName);
			sqlCommand.Parameters.AddWithValue("@fNameValue", fName);
			sqlCommand.Parameters.AddWithValue("@lNameValue", lName);
			sqlCommand.Parameters.AddWithValue("@emailValue", email);
			sqlCommand.Parameters.AddWithValue("@passwordValue", password);
			sqlCommand.Parameters.AddWithValue("@userTypeValue", userType);

			sqlConnection.Open();
			//we're using a try/catch so that if the query errors out we can handle it gracefully
			//by closing the connection and moving on
			try
			{
				sqlCommand.ExecuteNonQuery();
				//int account_ID = Convert.ToInt32(sqlCommand.ExecuteScalar());
				//sqlCommand.ExecuteScalar();
				//here, you could use this accountID for additional queries regarding
				//the requested account.  Really this is just an example to show you
				//a query where you get the primary key of the inserted row back from
				//the database!
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		[WebMethod(true)]
		public void InsertMatchingResponses(string userName, string q1, string q2, string q3, string q4)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			string sqlSelect = "INSERT INTO responses (userName, q1, q2, q3, q4) " +
				"VALUES (@userNameValue, @q1Value, @q2Value, @q3Value, @q4Value); ";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@userNameValue", userName);
			sqlCommand.Parameters.AddWithValue("@q1Value", q1);
			sqlCommand.Parameters.AddWithValue("@q2Value", q2);
			sqlCommand.Parameters.AddWithValue("@q3Value", q3);
			sqlCommand.Parameters.AddWithValue("@q4Value", q4);

			sqlConnection.Open();
			try
			{
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		[WebMethod(true)]
		public void InsertResponseValues(string userName, string responseId)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			string sqlSelect = "INSERT INTO user_responses_table (userName, responseId) " +
				"VALUES (@userNameValue, @responseIdValue); ";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@userNameValue", userName);
			sqlCommand.Parameters.AddWithValue("@responseIdValue", responseId);

			sqlConnection.Open();
			try
			{
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		[WebMethod]
		public string GetMatches(string userName)
		{
			//DataTable sqlDt = new DataTable("matches");
			Matches[] allRecords = null;
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			string sqlSelect = "select eu.userName as 'Mentee', ru.userName as 'Mentor', eu.email as 'Mentee Email', ru.email as 'Mentor Email',count(*) as Commonality " +
							   "from user_table eu " +
							   "join user_responses_table eus on eu.userName = eus.userName " +
							   "join responses_table es on eus.responseId = es.responseId " +
							   "join user_responses_table rus on eus.responseId = rus.responseId " +
							   "join user_table ru on rus.userName = ru.userName " +
							   "where eu.userType = 1 " +
								"and eu.userName != ru.userName " +
								"and eu.userName = @userNameValue " +
								"group by eu.userName, ru.userName " +
								"order by eu.userName, ru.userName; ";
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
			sqlCommand.Parameters.AddWithValue("@userNameValue", userName);
			using (var sqlcommand = new MySqlCommand(sqlSelect, sqlConnection))
			{
				sqlConnection.Open();
				using (var reader = sqlCommand.ExecuteReader())
				{
					var matchesList = new List<Matches>();
					while (reader.Read())
						matchesList.Add(new Matches
						{
							mentee = reader.GetString(0),
							mentor = reader.GetString(1),
							menteeEmail = reader.GetString(2),
							mentorEmail = reader.GetString(3),
							commonality = reader.GetInt32(4)
						});
					allRecords = matchesList.ToArray();
					Array.ForEach(allRecords, Console.WriteLine);
				}
			}

			string jsonText = JsonConvert.SerializeObject(allRecords, Formatting.Indented);

			return jsonText;
		}

		//EXAMPLE OF A SELECT, AND RETURNING "COMPLEX" DATA TYPES
		[WebMethod(EnableSession = true)]
		//[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		public void GetAccounts()
		{
			//check out the return type. It's an array of Account objects. You can look at our custom Account class in this solution to see that it's 
			//just a container for public class-level variables. It's a simple container that asp.net will have no trouble converting into json. When we return
			//sets of information, it's a good idea to create a custom container class to represent instances (or rows) of that information, and then return an array of those objects.  
			//Keeps everything simple.
			//LOGIC: get all the active accounts and return them!
			DataTable sqlDt = new DataTable("grades");
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			string sqlSelect = "select Year, Term, Total_Credits, user_ID, CurrentGPA from Grades order by user_ID";
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
			//gonna use this to fill a data table
			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			//filling the data table
			sqlDa.Fill(sqlDt);

		}

		//EXAMPLE OF AN UPDATE QUERY WITH PARAMS PASSED IN
		[WebMethod]
		public void UpdateAccount(string userName, string fName, string lName, string email, string password, int userType)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			//this is a simple update, with parameters to pass in values
			string sqlSelect = "update user_table set userName=@userNameValue, fName=@fNameValue, lName=@lNameVaule, email=@emailValue, password=@passwordValue, userType=@userTypeValue" +
				"where userName = @userNameValue";
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
			sqlCommand.Parameters.AddWithValue("@userNameVaule", HttpUtility.UrlDecode(userName));
			sqlCommand.Parameters.AddWithValue("@fNameValue", HttpUtility.UrlDecode(fName));
			sqlCommand.Parameters.AddWithValue("@lNameValue", HttpUtility.UrlDecode(lName));
			sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));
			sqlCommand.Parameters.AddWithValue("@passwordValue", HttpUtility.UrlDecode(password));
			sqlCommand.Parameters.AddWithValue("@userTypeValue", userType); // need to find a fix for this!

			sqlConnection.Open();
			//we're using a try/catch so that if the query errors out we can handle it gracefully
			//by closing the connection and moving on
			try
			{
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		[WebMethod(EnableSession = true)]
		public Account[] GetAccountRequests(string userName)
		{
			DataTable sqlDt = new DataTable("user_table");

			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			string sqlSelect = "SELECT * FROM user_table WHERE userName = @userNameVaule";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@userNameValue", userName);

			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			sqlDa.Fill(sqlDt);

			List<Account> accountRequests = new List<Account>();
			for (int i = 0; i < sqlDt.Rows.Count; i++)
				accountRequests.Add(new Account
				{
					userName = sqlDt.Rows[i]["userName"].ToString(),
					fName = sqlDt.Rows[i]["fName"].ToString(),
					lName = sqlDt.Rows[i]["lName"].ToString(),
					email = sqlDt.Rows[i]["email"].ToString(),
					password = sqlDt.Rows[i]["password"].ToString(),
					userType = Convert.ToInt32(sqlDt.Rows[i]["userType"])
				}); 
			return accountRequests.ToArray();
		}


		//EXAMPLE OF A DELETE QUERY
		[WebMethod]
		public void DeleteAccount(string user_ID)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			//this is a simple update, with parameters to pass in values
			string sqlSelect = "delete from User_Accounts where user_ID=@idValue";
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
			//sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(user_ID));
			sqlCommand.Parameters.AddWithValue("@idValue", user_ID);
			sqlConnection.Open();
			try
			{
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		//EXAMPLE OF AN UPDATE QUERY
		[WebMethod(EnableSession = true)]
		public void ActivateAccount(string user_ID)
		{
			if (Convert.ToInt32(Session["AdminStatus"]) == 1)
			{
				string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
				//this is a simple update, with parameters to pass in values
				string sqlSelect = "update accounts set active=1 where id=@idValue";

				MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(user_ID));

				sqlConnection.Open();
				try
				{
					sqlCommand.ExecuteNonQuery();
				}
				catch (Exception e)
				{
				}
				sqlConnection.Close();
			}
		}
        [WebMethod(EnableSession = true)]
        public bool AnonEmail(string subject, string body, string recipient)
        {
            //DataTable sqlDt = new DataTable("Accounts");
            //string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
            //string sqlSelect = "select userName, email, from user_table where userName=@idValue order by userName";
            //MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            //MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
            ////gonna use this to fill a data table
            //sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(userName));
            //MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            ////filling the data table
            //sqlDa.Fill(sqlDt);

            bool trueFalse = false;
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;

            //smtpClient.Timeout = 10000;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("menteeanonymous@gmail.com", "MentorMatcher1!");

            MailMessage mailMsg = new MailMessage();
            mailMsg.From = new MailAddress("menteeanonymous@gmail.com");
            mailMsg.To.Add(recipient);
            //mailMsg.CC.Add("cc@ccServer.com");
            //mailMsg.Bcc.Add("bcc@bccServer.com");
            mailMsg.Subject = subject;
            mailMsg.Body = body;
            smtpClient.Send(mailMsg);
            trueFalse = true;
            return trueFalse;
            //Console.WriteLine("Mail sent");
        }
        //EXAMPLE OF A DELETE QUERY
        [WebMethod(EnableSession = true)]
		public void RejectAccount(string user_ID)
		{
			if (Convert.ToInt32(Session["AdminStatus"]) == 1)
			{
				string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
				string sqlSelect = "delete from accounts where id=@idValue";

				MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(user_ID));

				sqlConnection.Open();
				try
				{
					sqlCommand.ExecuteNonQuery();
				}
				catch (Exception e)
				{
				}
				sqlConnection.Close();
			}
		}

		[WebMethod(EnableSession = true)]
		public accountmanager.Account[] GetAccountData(string userName)
		{
			//check out the return type. It's an array of Account objects. You can look at our custom Account class in this solution to see that it's 
			//just a container for public class-level variables. It's a simple container that asp.net will have no trouble converting into json. When we return
			//sets of information, it's a good idea to create a custom container class to represent instances (or rows) of that information, and then return an array of those objects.  
			//Keeps everything simple.
			//LOGIC: get all the active accounts and return them!
			DataTable sqlDt = new DataTable("Accounts");
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			string sqlSelect = "select userName, fName, lName, email, password, userType from user_table where userName=@idValue order by userName";
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
            //gonna use this to fill a data table
            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(userName));
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			//filling the data table
			sqlDa.Fill(sqlDt);
			//loop through each row in the dataset, creating instances
			//of our container class Account. Fill each acciount with
			//data from the rows, then dump them in a list.
			List<Account> Accounts = new List<Account>();
			for (int i = 0; i < sqlDt.Rows.Count; i++)
			{
				Accounts.Add(new Account
				{
					userName = sqlDt.Rows[i]["userName"].ToString(),
					fName = sqlDt.Rows[i]["fName"].ToString(),
					lName = sqlDt.Rows[i]["lName"].ToString(),
					email = sqlDt.Rows[i]["email"].ToString(),
					password = sqlDt.Rows[i]["password"].ToString(),
					userType = Convert.ToInt32(sqlDt.Rows[i]["userType"])
				});
			}
            //convert the list of accounts to an array and return!
            //return User_Accounts.ToArray();
            //System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            //this.Context.Response.ContentType = "application/json; charset=utf-8";
            //this.Context.Response.Write(jss.Serialize(Accounts.ToArray()));
            return Accounts.ToArray();
		}
    

        [WebMethod(true)]
		public void UpdateUserAccount(string userName, string fName, string lName, string email, string password, int userType)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			//the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
			//does is tell mySql server to return the primary key of the last inserted row.
			string sqlSelect = "UPDATE user_table SET userName=@userNameValue, fName=@fNameValue, lName=@lNameValue, email=@emailValue, password=@passwordValue, userType=@userTypeValue WHERE userName=@userNameValue";
			//"SELECT LAST_INSERT_ID();";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@userNameValue", userName);
			sqlCommand.Parameters.AddWithValue("@fNameValue", fName);
			sqlCommand.Parameters.AddWithValue("@lNameValue", lName);
			sqlCommand.Parameters.AddWithValue("@emailValue", email);
			sqlCommand.Parameters.AddWithValue("@passwordValue", password);
			sqlCommand.Parameters.AddWithValue("@userTypeValue", userType);

			//filling the data table
			//convert the list of accounts to an array and return!

			//this time, we're not using a data adapter to fill a data table.  We're just
			//opening the connection, telling our command to "executescalar" which says basically
			//execute the query and just hand me back the number the query returns (the ID, remember?).
			//don't forget to close the connection!

			sqlConnection.Open();
			//we're using a try/catch so that if the query errors out we can handle it gracefully
			//by closing the connection and moving on
			try
			{
				sqlCommand.ExecuteNonQuery();
				//int account_ID = Convert.ToInt32(sqlCommand.ExecuteScalar());
				//sqlCommand.ExecuteScalar();
				//here, you could use this accountID for additional queries regarding
				//the requested account.  Really this is just an example to show you
				//a query where you get the primary key of the inserted row back from
				//the database!
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();

		}

		[WebMethod(true)]
		public void InsertGoals(string userName, string myGoal, int goalStatus)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			string sqlSelect = "INSERT INTO goals (userName, myGoal, goalStatus) VALUES (@userNameValue, @myGoalValue, @goalStatusValue);";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@userNameValue", userName);
			sqlCommand.Parameters.AddWithValue("@myGoalValue", myGoal);
			sqlCommand.Parameters.AddWithValue("@goalStatusValue", goalStatus);

			sqlConnection.Open();
			try
			{
				sqlCommand.ExecuteNonQuery();
			}
			catch(Exception e)
			{
				sqlConnection.Close();
			}
		}

		[WebMethod(true)]
		public void UpdateMenteeMatch(string userName, string match)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			string sqlSelect = "UPDATE user_table SET user_table.match = @matchValue WHERE userName = @userNameValue";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@userNameValue", userName);
			sqlCommand.Parameters.AddWithValue("@matchValue", match);

			sqlConnection.Open();
			try
			{
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				sqlConnection.Close();
			}
		}

		[WebMethod(true)]
		public void UpdateMentorMatch(string match, string userName)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			string sqlSelect = "UPDATE user_table SET user_table.match = @matchValue WHERE userName = @userNameValue";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@userNameValue", match);
			sqlCommand.Parameters.AddWithValue("@matchValue", userName);

			sqlConnection.Open();
			try
			{
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				sqlConnection.Close();
			}
		}
	}
}