using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using FirebirdSql.Data.FirebirdClient;


namespace mojoPortal.Data
{
    /// <summary>
    /// Author:					Joe Audette
    /// Created:				2007-11-03
    /// Last Modified:			2008-02-02
    /// 
    /// The use and distribution terms for this software are covered by the 
    /// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)  
    /// which can be found in the file CPL.TXT at the root of this distribution.
    /// By using this software in any fashion, you are agreeing to be bound by 
    /// the terms of this license.
    ///
    /// You must not remove this notice, or any other, from this software.
    /// 
    /// Note moved into separate class file from dbPortal 2007-11-03
    /// 
    /// </summary>
    public static class DBSitePersonalization
    {
        
        public static String DBPlatform()
        {
            return "FirebirdSql";
        }

        private static String GetConnectionString()
        {
            return ConfigurationManager.AppSettings["FirebirdConnectionString"];

        }

       

        public static int GetCountOfState(
            int siteId,
            String path,
            bool allUserScope,
            Guid userGuid,
            DateTime inactiveSince)
        {
            int result = 0;

            Guid pathID = Guid.Empty;
            if ((path != null) && (path.Length > 0))
            {
                pathID = GetOrCreatePathId(siteId, path);
            }


            if (allUserScope)
            {
                if (pathID != Guid.Empty)
                {
                    result = GetCountOfStateAllUsers(pathID);
                }
                else
                {
                    result = GetCountOfStateAllUsers();
                }

            }
            else
            {

                if (userGuid != Guid.Empty)
                {
                    if (pathID != Guid.Empty)
                    {
                        if (inactiveSince > DateTime.MinValue)
                        {
                            result = GetCountOfStateByUser(userGuid, pathID, inactiveSince);

                        }
                        else
                        {
                            result = GetCountOfStateByUser(userGuid, pathID);
                        }
                    }
                    else
                    {
                        if (inactiveSince > DateTime.MinValue)
                        {
                            result = GetCountOfStateByUserAllPaths(userGuid, inactiveSince);

                        }
                        else
                        {
                            result = GetCountOfStateByUserAllPaths(userGuid);
                        }

                    }

                }
                else
                {
                    // not a specific user
                    if (pathID != Guid.Empty)
                    {
                        if (inactiveSince > DateTime.MinValue)
                        {
                            result = GetCountOfStateByUser(pathID, inactiveSince);

                        }
                        else
                        {
                            result = GetCountOfStateByUser(inactiveSince);
                        }
                    }
                    else
                    {
                        // not a specific path
                        if (inactiveSince > DateTime.MinValue)
                        {
                            result = GetCountOfStateByUser(inactiveSince);

                        }
                        else
                        {
                            result = GetCountOfStateByUser();
                        }

                    }

                }

            }

            return result;
        }

        public static int GetCountOfStateByUser(
            Guid userGuid,
            Guid pathId)
        {

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT Count(*) ");
            sqlCommand.Append("FROM	mp_SitePersonalizationPerUser ");
            sqlCommand.Append("WHERE PathID = @PathID AND UserID = @UserID ; ");

            FbParameter[] arParams = new FbParameter[2];

            arParams[0] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pathId.ToString();

            arParams[1] = new FbParameter("@UserID", FbDbType.VarChar, 36);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = userGuid.ToString();

            int count = Convert.ToInt32(FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams));

            return count;


        }

        public static int GetCountOfStateByUser(
            Guid userGuid,
            Guid pathId,
            DateTime inactiveSinceTime)
        {

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT Count(pu.*) ");
            sqlCommand.Append("FROM	mp_SitePersonalizationPerUser pu ");
            sqlCommand.Append("JOIN	mp_Users u ");
            sqlCommand.Append("ON	pu.UserID = u.UserGuid ");
            sqlCommand.Append("WHERE pu.PathID = @PathID AND pu.UserID = @UserID  ");
            sqlCommand.Append("AND u.LastActivityDate <= @LastActivityDate ; ");

            FbParameter[] arParams = new FbParameter[3];

            arParams[0] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pathId.ToString();

            arParams[1] = new FbParameter("@UserID", FbDbType.VarChar, 36);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = userGuid.ToString();

            arParams[2] = new FbParameter("@LastActivityDate", FbDbType.TimeStamp);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = inactiveSinceTime;

            int count = Convert.ToInt32(FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams));

            return count;


        }

        public static int GetCountOfStateByUser(
            Guid pathId,
            DateTime inactiveSinceTime)
        {

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT Count(pu.*) ");
            sqlCommand.Append("FROM	mp_SitePersonalizationPerUser pu ");
            sqlCommand.Append("JOIN	mp_Users u ");
            sqlCommand.Append("ON	pu.UserID = u.UserGuid ");
            sqlCommand.Append("WHERE pu.PathID = @PathID   ");
            sqlCommand.Append("AND u.LastActivityDate <= @LastActivityDate ; ");

            FbParameter[] arParams = new FbParameter[2];

            arParams[0] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pathId.ToString();

            arParams[1] = new FbParameter("@LastActivityDate", FbDbType.TimeStamp);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = inactiveSinceTime;

            int count = Convert.ToInt32(FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams));

            return count;


        }

        public static int GetCountOfStateByUserAllPaths(
            Guid userGuid,
            DateTime inactiveSinceTime)
        {

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT Count(pu.*) ");
            sqlCommand.Append("FROM	mp_SitePersonalizationPerUser pu ");
            sqlCommand.Append("JOIN	mp_Users u ");
            sqlCommand.Append("ON	pu.UserID = u.UserGuid ");
            sqlCommand.Append("WHERE pu.UserID = @UserID   ");
            sqlCommand.Append("AND u.LastActivityDate <= @LastActivityDate ; ");

            FbParameter[] arParams = new FbParameter[2];

            arParams[0] = new FbParameter("@UserID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = userGuid.ToString();

            arParams[1] = new FbParameter("@LastActivityDate", FbDbType.TimeStamp);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = inactiveSinceTime;

            int count = Convert.ToInt32(FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams));

            return count;


        }

        public static int GetCountOfStateByUserAllPaths(Guid userGuid)
        {

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT Count(pu.*) ");
            sqlCommand.Append("FROM	mp_SitePersonalizationPerUser pu ");
            sqlCommand.Append("JOIN	mp_Users u ");
            sqlCommand.Append("ON	pu.UserID = u.UserGuid ");
            sqlCommand.Append("WHERE pu.UserID = @UserID ;  ");

            FbParameter[] arParams = new FbParameter[1];

            arParams[0] = new FbParameter("@UserID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = userGuid.ToString();

            int count = Convert.ToInt32(FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams));

            return count;

        }

        public static int GetCountOfStateByUser(DateTime inactiveSinceTime)
        {

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT Count(pu.*) ");
            sqlCommand.Append("FROM	mp_SitePersonalizationPerUser pu ");
            sqlCommand.Append("JOIN	mp_Users u ");
            sqlCommand.Append("ON	pu.UserID = u.UserGuid ");
            sqlCommand.Append("WHERE    ");
            sqlCommand.Append(" u.LastActivityDate <= @LastActivityDate ; ");

            FbParameter[] arParams = new FbParameter[1];

            arParams[0] = new FbParameter("@LastActivityDate", FbDbType.TimeStamp);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = inactiveSinceTime;

            int count = Convert.ToInt32(FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams));

            return count;


        }

        public static int GetCountOfStateByUser()
        {

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT Count(pu.*) ");
            sqlCommand.Append("FROM	mp_SitePersonalizationPerUser pu ;");

            int count = Convert.ToInt32(FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString()));

            return count;


        }

        public static int GetCountOfStateAllUsers(Guid pathId)
        {

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT Count(*) ");
            sqlCommand.Append("FROM	mp_SitePersonalizationAllUsers ");
            sqlCommand.Append("WHERE PathID = @PathID  ; ");

            FbParameter[] arParams = new FbParameter[1];

            arParams[0] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pathId.ToString();

            int count = Convert.ToInt32(FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams));

            return count;

        }

        public static int GetCountOfStateAllUsers()
        {

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT Count(*) ");
            sqlCommand.Append("FROM	mp_SitePersonalizationAllUsers ");
            sqlCommand.Append("  ; ");

            int count = Convert.ToInt32(FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString()));

            return count;

        }

        public static void SavePersonalizationBlob(
            int siteId,
            String path,
            Guid userGuid,
            byte[] dataBlob,
            DateTime lastUpdateTime)
        {
            Guid pathID = GetOrCreatePathId(siteId, path);
            if (PersonalizationBlobExists(pathID, userGuid))
            {
                UpdatePersonalizationBlob(userGuid, pathID, dataBlob, lastUpdateTime);
            }
            else
            {
                CreatePersonalizationBlob(userGuid, pathID, dataBlob, lastUpdateTime);
            }


        }

        public static byte[] GetPersonalizationBlob(
            int siteId,
            String path,
            Guid userGuid)
        {
            Guid pathID = GetOrCreatePathId(siteId, path);

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT FIRST 1 PageSettings ");
            sqlCommand.Append("FROM	mp_SitePersonalizationPerUser ");
            sqlCommand.Append("WHERE PathID = @PathID AND UserID = @UserID  ; ");

            FbParameter[] arParams = new FbParameter[2];

            arParams[0] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pathID.ToString();

            arParams[1] = new FbParameter("@UserID", FbDbType.VarChar, 36);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = userGuid.ToString();

            byte[] result = null;

            //try
            //{
            result = (byte[])FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);
            //}
            //catch (System.InvalidCastException ex)
            //{
            //    if (log.IsErrorEnabled)
            //    {
            //        log.Error("dbPortal.SitePersonalization_GetPersonalizationBlob", ex);
            //    }
            //}

            return result;

        }

        public static void ResetPersonalizationBlob(
            int siteId,
            String path,
            Guid userGuid)
        {
            Guid pathID = GetOrCreatePathId(siteId, path);

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE  ");
            sqlCommand.Append("FROM	mp_SitePersonalizationPerUser ");
            sqlCommand.Append("WHERE PathID = @PathID AND UserID = @UserID  ; ");

            FbParameter[] arParams = new FbParameter[2];

            arParams[0] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pathID.ToString();

            arParams[1] = new FbParameter("@UserID", FbDbType.VarChar, 36);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = userGuid.ToString();


            FBSqlHelper.ExecuteNonQuery(
                    GetConnectionString(),
                    sqlCommand.ToString(),
                    arParams);


        }

        public static void ResetPersonalizationBlob(
            int siteId,
            String path)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE  ");
            sqlCommand.Append("FROM	mp_SitePersonalizationAllUsers ");
            sqlCommand.Append("WHERE PathID IN (   ");
            sqlCommand.Append("SELECT PathID    ");
            sqlCommand.Append("FROM mp_SitePaths    ");
            sqlCommand.Append("WHERE SiteID = @SiteID  ");
            sqlCommand.Append(" )  ");
            sqlCommand.Append("  ; ");

            FbParameter[] arParams = new FbParameter[2];

            arParams[0] = new FbParameter("@SiteID", FbDbType.Integer);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = siteId;


            FBSqlHelper.ExecuteNonQuery(
                    GetConnectionString(),
                    sqlCommand.ToString(),
                    arParams);

        }

        public static byte[] GetPersonalizationBlobAllUsers(
            int siteId,
            String path)
        {
            Guid pathID = GetOrCreatePathId(siteId, path);

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT FIRST 1 PageSettings ");
            sqlCommand.Append("FROM	mp_SitePersonalizationAllUsers ");
            sqlCommand.Append("WHERE PathID = @PathID  ; ");

            FbParameter[] arParams = new FbParameter[1];

            arParams[0] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pathID.ToString();


            byte[] result = null;

            //try
            //{
            result = (byte[])FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);
            //}
            //catch (System.InvalidCastException ex)
            //{
            //    if (log.IsErrorEnabled)
            //    {
            //        log.Error("dbPortal.SitePersonalization_GetPersonalizationBlobAllUsers", ex);
            //    }
            //}

            return result;

        }

        public static void SavePersonalizationBlobAllUsers(
            int siteId,
            String path,
            byte[] dataBlob,
            DateTime lastUpdateTime)
        {
            Guid pathID = GetOrCreatePathId(siteId, path);

            if (PersonalizationBlobExists(pathID))
            {
                UpdatePersonalizationBlob(pathID, dataBlob, lastUpdateTime);
            }
            else
            {
                CreatePersonalizationBlob(pathID, dataBlob, lastUpdateTime);
            }

        }

        public static void CreatePersonalizationBlob(
            Guid userGuid,
            Guid pathId,
            byte[] dataBlob,
            DateTime lastUpdateTime)
        {

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("INSERT INTO mp_SitePersonalizationPerUser (ID, UserID, PathID, PageSettings, LastUpdate)");
            sqlCommand.Append("VALUES (");
            sqlCommand.Append(" @ID , ");
            sqlCommand.Append(" @UserID , ");
            sqlCommand.Append(" @PathID , ");
            sqlCommand.Append(" @PageSettings , ");
            sqlCommand.Append(" @LastUpdate  ");
            sqlCommand.Append(");");


            FbParameter[] arParams = new FbParameter[5];

            arParams[0] = new FbParameter("@ID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = Guid.NewGuid().ToString(); ;


            arParams[1] = new FbParameter("@UserID", FbDbType.VarChar, 36);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = userGuid.ToString();

            arParams[2] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = pathId.ToString();

            arParams[3] = new FbParameter("@PageSettings", FbDbType.Binary);
            arParams[3].Direction = ParameterDirection.Input;
            arParams[3].Value = dataBlob;

            arParams[4] = new FbParameter("@LastUpdate", FbDbType.TimeStamp);
            arParams[4].Direction = ParameterDirection.Input;
            arParams[4].Value = lastUpdateTime;

            int rowsAffected = Convert.ToInt32(FBSqlHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams).ToString());

        }

        public static void CreatePersonalizationBlob(
            Guid pathId,
            byte[] dataBlob,
            DateTime lastUpdateTime)
        {

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("INSERT INTO mp_SitePersonalizationAllUsers (PathID, PageSettings, LastUpdate)");
            sqlCommand.Append("VALUES (");
            sqlCommand.Append(" @PathID , ");
            sqlCommand.Append(" @PageSettings , ");
            sqlCommand.Append(" @LastUpdate  ");
            sqlCommand.Append(");");

            FbParameter[] arParams = new FbParameter[3];

            arParams[0] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pathId.ToString();

            arParams[1] = new FbParameter("@PageSettings", FbDbType.Binary);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = dataBlob;

            arParams[2] = new FbParameter("@LastUpdate", FbDbType.TimeStamp);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = lastUpdateTime;

            int rowsAffected = Convert.ToInt32(FBSqlHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams).ToString());


        }

        public static void UpdatePersonalizationBlob(
            Guid userGuid,
            Guid pathId,
            byte[] dataBlob,
            DateTime lastUpdateTime)
        {

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("UPDATE mp_SitePersonalizationPerUser ");
            sqlCommand.Append("SET ");
            sqlCommand.Append("PageSettings = @PageSettings, ");
            sqlCommand.Append("LastUpdate = @LastUpdate ");

            sqlCommand.Append("WHERE UserID = @UserID AND PathID = @PathID  ;");

            FbParameter[] arParams = new FbParameter[4];

            arParams[0] = new FbParameter("@UserID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = userGuid.ToString();

            arParams[1] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = pathId.ToString();

            arParams[2] = new FbParameter("@PageSettings", FbDbType.Binary);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = dataBlob;

            arParams[3] = new FbParameter("@LastUpdate", FbDbType.TimeStamp);
            arParams[3].Direction = ParameterDirection.Input;
            arParams[3].Value = lastUpdateTime;

            int rowsAffected = 0;

            rowsAffected = FBSqlHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);


        }

        public static void UpdatePersonalizationBlob(
            Guid pathId,
            byte[] dataBlob,
            DateTime lastUpdateTime)
        {

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("UPDATE mp_SitePersonalizationAllUsers ");
            sqlCommand.Append("SET ");
            sqlCommand.Append("PageSettings = @PageSettings, ");
            sqlCommand.Append("LastUpdate = @LastUpdate ");

            sqlCommand.Append("WHERE PathID = @PathID  ;");

            FbParameter[] arParams = new FbParameter[3];

            arParams[0] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pathId.ToString();

            arParams[1] = new FbParameter("@PageSettings", FbDbType.Binary);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = dataBlob;

            arParams[2] = new FbParameter("@LastUpdate", FbDbType.TimeStamp);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = lastUpdateTime;

            int rowsAffected = 0;

            rowsAffected = FBSqlHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

        }

        public static bool PersonalizationBlobExists(Guid pathId, Guid userGuid)
        {
            bool result = false;

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT Count(*) ");
            sqlCommand.Append("FROM	mp_SitePersonalizationPerUser ");
            sqlCommand.Append("WHERE PathID = @PathID AND UserID = @UserID ; ");

            FbParameter[] arParams = new FbParameter[2];

            arParams[0] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pathId.ToString();

            arParams[1] = new FbParameter("@UserID", FbDbType.VarChar, 36);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = userGuid.ToString();

            int count = Convert.ToInt32(FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams));

            result = (count > 0);


            return result;
        }

        public static bool PersonalizationBlobExists(Guid pathId)
        {
            bool result = false;

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT Count(*) ");
            sqlCommand.Append("FROM	mp_SitePersonalizationAllUsers ");
            sqlCommand.Append("WHERE PathID = @PathID  ; ");

            FbParameter[] arParams = new FbParameter[1];

            arParams[0] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pathId.ToString();

            int count = Convert.ToInt32(FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams));

            result = (count > 0);


            return result;
        }

        public static Guid GetOrCreatePathId(int siteId, String path)
        {
            Guid result = Guid.Empty;

            if (PathExists(siteId, path))
            {
                result = GetPathId(siteId, path);
            }
            else
            {
                result = CreatePath(siteId, path);
            }

            return result;
        }

        public static Guid GetPathId(int siteId, String path)
        {
            Guid result = Guid.Empty;

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT FIRST 1 PathID ");
            sqlCommand.Append("FROM	mp_SitePaths ");
            sqlCommand.Append("WHERE SiteID = @SiteID AND LoweredPath = @LoweredPath  ; ");

            FbParameter[] arParams = new FbParameter[2];

            arParams[0] = new FbParameter("@SiteID", FbDbType.Integer);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = siteId;

            arParams[1] = new FbParameter("@LoweredPath", FbDbType.VarChar, 255);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = path.ToLower();

            string guidString = FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams).ToString();

            result = new Guid(guidString);


            return result;
        }

        public static bool PathExists(int siteId, String path)
        {
            bool result = false;

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT Count(*) ");
            sqlCommand.Append("FROM	mp_SitePaths ");
            sqlCommand.Append("WHERE SiteID = @SiteID AND LoweredPath = @LoweredPath ; ");

            FbParameter[] arParams = new FbParameter[2];

            arParams[0] = new FbParameter("@SiteID", FbDbType.Integer);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = siteId;

            arParams[1] = new FbParameter("@LoweredPath", FbDbType.VarChar, 255);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = path.ToLower();

            int count = Convert.ToInt32(FBSqlHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams));

            result = (count > 0);


            return result;
        }

        public static Guid CreatePath(int siteId, String path)
        {
            Guid newPathID = Guid.NewGuid();
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("INSERT INTO mp_SitePaths (PathID, SiteID, SiteGuid, Path, LoweredPath)");
            sqlCommand.Append("VALUES (");
            sqlCommand.Append(" @PathID , ");
            sqlCommand.Append(" @SiteID , ");
            sqlCommand.Append(" (SELECT FIRST 1 SiteGuid FROM mp_Sites WHERE SiteID = @SiteID) , ");
            sqlCommand.Append(" @Path , ");
            sqlCommand.Append(" @LoweredPath  ");
            sqlCommand.Append(");");


            FbParameter[] arParams = new FbParameter[4];

            arParams[0] = new FbParameter("@PathID", FbDbType.VarChar, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = newPathID.ToString();

            arParams[1] = new FbParameter("@SiteID", FbDbType.Integer);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = siteId;

            arParams[2] = new FbParameter("@Path", FbDbType.VarChar, 255);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = path;

            arParams[3] = new FbParameter("@LoweredPath", FbDbType.VarChar, 255);
            arParams[3].Direction = ParameterDirection.Input;
            arParams[3].Value = path.ToLower();

            int rowsAffected = Convert.ToInt32(FBSqlHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams).ToString());

            return newPathID;

        }

        

    }
}
