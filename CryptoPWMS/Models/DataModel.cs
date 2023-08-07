﻿namespace CryptoPWMS.Models
{
    /*
     *   ________          __              _____             .___     .__   
     *   \______ \ _____ _/  |______      /     \   ____   __| _/____ |  |  
     *    |    |  \\__  \\   __\__  \    /  \ /  \ /  _ \ / __ |/ __ \|  |  
     *    |    `   \/ __ \|  |  / __ \_ /    Y    (  <_> ) /_/ \  ___/|  |__
     *   /_______  (____  /__| (____  / \____|__  /\____/\____ |\___  >____/
     *           \/     \/          \/          \/            \/    \/      
     */

    /// <summary>
    /// Datamodel of the User entity in the database.
    /// </summary>
    public class User
    {
        public int Id { get; set; }                 // Primary key, Autoincrements in DB.
        public string Username { get; set; }        
        public string Password { get; set; }
    }

    /// <summary>
    /// Datamodel of the PasswordGroup entity in the Database.
    /// </summary>
    public class PasswordGroup
    {
        public int Id { get; set; }                 // Primary key, Autoincrements in DB.
        public string Name { get; set; }
    }

    /// <summary>
    /// Datamodel of the Password entity in the database. 
    /// </summary>
    public class PasswordItem
    {
        public int Id { get; set; }                 // Primary key, Autoincrements in DB.
        public int User_Id { get; set; }            // Foreign key - References User.Id.
        public int Grp_Id { get; set; }             // Foreign key - References PasswordGroup.Id.
        public string Platform { get; set; }        
        public string URL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Key { get; set; }
        public string salt { get; set; }
        public string LastUpdated { get; set; }
    }
}
