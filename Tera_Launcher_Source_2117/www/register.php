<?php
include "config.php"; //the database configuration file. Update this to connect to your databse

if(isset($_POST['username'] , $_POST['password'] , $_POST['email']))
{
			$username = $_POST['username'];
			$enc_password = md5($_POST['password']);
			$email = $_POST['email'];

			mysql_real_escape_string($username);
			mysql_real_escape_string($enc_password);
			mysql_real_escape_string($email);

 $email_check = mysql_query("SELECT AccountEmail FROM Accounts WHERE AccountEmail = '".$email."'");
 $email_exists = mysql_num_rows($email_check);
 $user_check = mysql_query("SELECT AccountName FROM Accounts WHERE AccountName = '".$username."'");
 $user_exists = mysql_num_rows($user_check);
 
 if($email_exists > 0 || $user_exists > 0 )
 {
  echo "In use";
 }
 else
	{
        //find the next userid
        $query="select max(AccountId) as total from Accounts";
        $results=mysql_query($query) or die(mysql_error());
        if($results)
		{
                $row=mysql_fetch_row($results);//store the result in $row array
                $user_id=++$row['0'];//increment the current user_id by 1
        }
        mysql_free_result($results);//free the result
   
		$query="INSERT INTO Accounts(AccountId,AccountName,AccountPassword,AccountEmail,AccessLevel,Membership,LastOnlineUtc,Coins,UISettings)";
        $query=$query."values('".$user_id."','".$username."','".$enc_password."','".$email."','"."0"."','"."20001"."','"."0"."','"."0"."','".""."')";

        $results=mysql_query($query) or die(mysql_error());
		 
		if($results)
		{
		echo 'registered succesfully!';
        return 1;
		}
	}
}
else
{
  echo 'register failed!';
}

?>