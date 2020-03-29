//and we need this to manipulate data from a db
using System.Data;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
//we need these to talk to mysql
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

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

		[WebMethod]
		public bool LogOn(string user_ID, string pass)
		{
			//LOGIC: pass the parameters into the database to see if an account
			//with these credentials exist.  If it does, then return true.  If
			//it doesn't, then return false
			//we return this flag to tell them if they logged in or not
			bool success = false;
			//our connection string comes from our web.config file like we talked about earlier
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			//here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
			string sqlSelect = "SELECT user_ID FROM User_Accounts WHERE user_ID=@idValue and Password=@passValue";
			//set up our connection object to be ready to use our connection string
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			//set up our command object to use our connection, and our query
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
			//tell our command to replace the @parameters with real values
			//we decode them because they came to us via the web so they were encoded
			//for transmission (funky characters escaped, mostly)
			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(user_ID));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));
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
		public void RequestAccount(string user_ID, string University_name, string Fname, string Lname, string Password, string Major, int AdminStatus)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			//the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
			//does is tell mySql server to return the primary key of the last inserted row.
			string sqlSelect = "INSERT INTO User_Accounts (user_ID, University_name, Fname, Lname, Password, Major, AdminStatus) " +
				"VALUES (@user_IDValue, @University_nameValue, @FnameValue, @LnameValue, @PasswordValue, @MajorValue, @AdminStatusValue); ";
			//"SELECT LAST_INSERT_ID();";
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@user_IDValue", user_ID);
			sqlCommand.Parameters.AddWithValue("@University_nameValue", University_name);
			sqlCommand.Parameters.AddWithValue("@FnameValue", Fname);
			sqlCommand.Parameters.AddWithValue("@LnameValue", Lname);
			sqlCommand.Parameters.AddWithValue("@PasswordValue", Password);
			sqlCommand.Parameters.AddWithValue("@MajorValue", Major);
			sqlCommand.Parameters.AddWithValue("@AdminStatusValue", AdminStatus);

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

		// METHOD TO HANDLE GRADE ENTRY DATA
		[WebMethod(EnableSession = true)]
		public void HandleGradeEntryData(int Year, string Term, int Total_Credits, double CurrentGPA)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;

			string sqlSelect = "INSERT INTO Grades (Year, Term, Total_Credits, CurrentGPA) " +
				"VALUES (@yearValue, @termValue, @totalCreditsValue, @currentGpaValue);";

			//"SELECT LAST_INSERT_ID();";
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@yearValue", Year);
			sqlCommand.Parameters.AddWithValue("@termValue", Term);
			sqlCommand.Parameters.AddWithValue("@totalCreditsValue", Total_Credits);
			sqlCommand.Parameters.AddWithValue("@currentGpaValue", CurrentGPA);

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
			//loop through each row in the dataset, creating instances
			//of our container class Account. Fill each acciount with
			//data from the rows, then dump them in a list.
			//List<Grade> grades = new List<Grade>();
			//for (int i = 0; i < sqlDt.Rows.Count; i++)
			//{
			//	grades.Add(new Grade
			//	{
			//		Year = Convert.ToInt32(sqlDt.Rows[i]["Year"]),
			//		user_ID = Convert.ToInt32(sqlDt.Rows[i]["user_ID"]),
			//		Term = sqlDt.Rows[i]["Term"].ToString(),
			//		Total_Credits = Convert.ToDouble(sqlDt.Rows[i]["Total_Credits"]),
			//		CurrentGPA = Convert.ToDouble(sqlDt.Rows[i]["CurrentGPA"]),
			//	});
			//}
			//convert the list of accounts to an array and return!
			//System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
			//this.Context.Response.ContentType = "application/json; charset=utf-8";
			//this.Context.Response.Write(jss.Serialize(grades.ToArray()));
		}

		//EXAMPLE OF AN UPDATE QUERY WITH PARAMS PASSED IN
		[WebMethod]
		public void UpdateAccount(string user_ID, string University_name, string Password, string Fname, string Lname, string Major)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			//this is a simple update, with parameters to pass in values
			string sqlSelect = "update User_Accounts set user_ID=@uidValue, University_name=@uname, Password=@passValue, Fname=@fnameValue, Lname=@lnameValue," +
				"Major=@majorValue where user_ID=@uidValue";
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
			sqlCommand.Parameters.AddWithValue("@uidValue", HttpUtility.UrlDecode(user_ID));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(Password));
			sqlCommand.Parameters.AddWithValue("@fnameValue", HttpUtility.UrlDecode(Fname));
			sqlCommand.Parameters.AddWithValue("@lnameValue", HttpUtility.UrlDecode(Lname));
			sqlCommand.Parameters.AddWithValue("@majorValue", HttpUtility.UrlDecode(Major));
			sqlCommand.Parameters.AddWithValue("@uname", HttpUtility.UrlDecode(University_name));
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
		public Account[] GetAccountRequests(string user_ID)
		{
			DataTable sqlDt = new DataTable("User_Accounts");

			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			string sqlSelect = "SELECT * FROM User_Accounts WHERE user_ID = @userIdValue";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@user_IDValue", user_ID);

			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			sqlDa.Fill(sqlDt);

			List<Account> accountRequests = new List<Account>();
			for (int i = 0; i < sqlDt.Rows.Count; i++)
				accountRequests.Add(new Account
				{
					user_ID = sqlDt.Rows[i]["user_ID"].ToString(),
					University_name = sqlDt.Rows[i]["University_name"].ToString(),
					Fname = sqlDt.Rows[i]["Fname"].ToString(),
					Lname = sqlDt.Rows[i]["Lname"].ToString(),
					Password = sqlDt.Rows[i]["Password"].ToString(),
					Major = sqlDt.Rows[i]["Major"].ToString(),
					AdminStatus = Convert.ToInt32(sqlDt.Rows[i]["AdminStatus"])
				});
			return accountRequests.ToArray();
		}
		//return GetAccountRequests.ToArray();


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
		public void GetAccountData()
		{
			//check out the return type. It's an array of Account objects. You can look at our custom Account class in this solution to see that it's 
			//just a container for public class-level variables. It's a simple container that asp.net will have no trouble converting into json. When we return
			//sets of information, it's a good idea to create a custom container class to represent instances (or rows) of that information, and then return an array of those objects.  
			//Keeps everything simple.
			//LOGIC: get all the active accounts and return them!
			DataTable sqlDt = new DataTable("Accounts");
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			string sqlSelect = "select user_ID, University_name, Fname, Lname, Password, Major from User_Accounts order by user_ID";
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
			//gonna use this to fill a data table
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
					//user_ID = Convert.ToInt32(sqlDt.Rows[i]["user_ID"]),
					user_ID = sqlDt.Rows[i]["user_ID"].ToString(),
					University_name = sqlDt.Rows[i]["University_name"].ToString(),
					Fname = sqlDt.Rows[i]["Fname"].ToString(),
					Lname = sqlDt.Rows[i]["Lname"].ToString(),
					Password = sqlDt.Rows[i]["Password"].ToString(),
					Major = sqlDt.Rows[i]["Major"].ToString(),
				});
			}
			//convert the list of accounts to an array and return!
			//return User_Accounts.ToArray();
			System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
			this.Context.Response.ContentType = "application/json; charset=utf-8";
			this.Context.Response.Write(jss.Serialize(Accounts.ToArray()));
		}

		[WebMethod(true)]
		public void UpdateUserAccount(string user_ID, string University_name, string Fname, string Lname, string Password, string Major)//, int AdminStatus)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["pentest"].ConnectionString;
			//the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
			//does is tell mySql server to return the primary key of the last inserted row.
			string sqlSelect = "UPDATE User_Accounts SET user_ID=@user_IDValue, University_name=@University_nameValue, Fname=@FnameValue, Lname=@LnameValue, Password=@PasswordValue, Major=@MajorValue WHERE user_ID = @user_IDValue";
			//"SELECT LAST_INSERT_ID();";
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
			//sqlConnection.Open();
			//sqlCommand.Parameters.AddWithValue("@user_IDValue", HttpUtility.UrlDecode(uid));
			//sqlCommand.Parameters.Add("@user_IDValue", uid);
			//sqlCommand.Parameters.AddWithValue("@user_IDValue", Convert.ToInt32(uid));
			sqlCommand.Parameters.AddWithValue("@user_IDValue", user_ID);
			sqlCommand.Parameters.AddWithValue("@University_nameValue", University_name);
			sqlCommand.Parameters.AddWithValue("@FnameValue", Fname);
			sqlCommand.Parameters.AddWithValue("@LnameValue", Lname);
			sqlCommand.Parameters.AddWithValue("@PasswordValue", Password);
			sqlCommand.Parameters.AddWithValue("@MajorValue", Major);
			//filling the data table
			//convert the list of accounts to an array and return!
			//return User_Accounts.ToArray();
			//sqlCommand.Parameters.AddWithValue("@user_IDValue", HttpUtility.UrlDecode(user_ID));
			//sqlCommand.Parameters.AddWithValue("@University_nameValue", HttpUtility.UrlDecode(University_name));
			//sqlCommand.Parameters.AddWithValue("@FnameValue", HttpUtility.UrlDecode(Fname));
			//sqlCommand.Parameters.AddWithValue("@LnameValue", HttpUtility.UrlDecode(Lname));
			//sqlCommand.Parameters.AddWithValue("@PasswordValue", HttpUtility.UrlDecode(Password));
			// sqlCommand.Parameters.AddWithValue("@MajorValue", HttpUtility.UrlDecode(Major));
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
	}
}