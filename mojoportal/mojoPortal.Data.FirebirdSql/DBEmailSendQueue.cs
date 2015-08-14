﻿/// Author:					Joe Audette
/// Created:				2009-03-01
/// Last Modified:			2009-03-01
/// 
/// The use and distribution terms for this software are covered by the 
/// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)  
/// which can be found in the file CPL.TXT at the root of this distribution.
/// By using this software in any fashion, you are agreeing to be bound by 
/// the terms of this license.
///
/// You must not remove this notice, or any other, from this software.

using System;
using System.Data;
using System.Configuration;
using System.Globalization;
using System.Text;
using FirebirdSql.Data.FirebirdClient;

namespace mojoPortal.Data
{
    public static class DBEmailSendQueue
    {
        private static String GetReadConnectionString()
        {
            return ConfigurationManager.AppSettings["FirebirdConnectionString"];

        }

        private static String GetWriteConnectionString()
        {
            if (ConfigurationManager.AppSettings["FirebirdWriteConnectionString"] != null)
            {
                return ConfigurationManager.AppSettings["FirebirdWriteConnectionString"];
            }

            return ConfigurationManager.AppSettings["FirebirdConnectionString"];

        }


        /// <summary>
        /// Inserts a row in the mp_EmailSendQueue table. Returns rows affected count.
        /// </summary>
        /// <param name="guid"> guid </param>
        /// <param name="siteGuid"> siteGuid </param>
        /// <param name="moduleGuid"> moduleGuid </param>
        /// <param name="userGuid"> userGuid </param>
        /// <param name="specialGuid1"> specialGuid1 </param>
        /// <param name="specialGuid2"> specialGuid2 </param>
        /// <param name="fromAddress"> fromAddress </param>
        /// <param name="replyTo"> replyTo </param>
        /// <param name="toAddress"> toAddress </param>
        /// <param name="ccAddress"> ccAddress </param>
        /// <param name="bccAddress"> bccAddress </param>
        /// <param name="subject"> subject </param>
        /// <param name="textBody"> textBody </param>
        /// <param name="htmlBody"> htmlBody </param>
        /// <param name="type"> type </param>
        /// <param name="dateToSend"> dateToSend </param>
        /// <param name="createdUtc"> createdUtc </param>
        /// <returns>int</returns>
        public static int Create(
            Guid guid,
            Guid siteGuid,
            Guid moduleGuid,
            Guid userGuid,
            Guid specialGuid1,
            Guid specialGuid2,
            string fromAddress,
            string replyTo,
            string toAddress,
            string ccAddress,
            string bccAddress,
            string subject,
            string textBody,
            string htmlBody,
            string type,
            DateTime dateToSend,
            DateTime createdUtc)
        {

            FbParameter[] arParams = new FbParameter[17];


            arParams[0] = new FbParameter("@Guid", FbDbType.Char, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = guid.ToString();

            arParams[1] = new FbParameter("@SiteGuid", FbDbType.Char, 36);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = siteGuid.ToString();

            arParams[2] = new FbParameter("@ModuleGuid", FbDbType.Char, 36);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = moduleGuid.ToString();

            arParams[3] = new FbParameter("@UserGuid", FbDbType.Char, 36);
            arParams[3].Direction = ParameterDirection.Input;
            arParams[3].Value = userGuid.ToString();

            arParams[4] = new FbParameter("@SpecialGuid1", FbDbType.Char, 36);
            arParams[4].Direction = ParameterDirection.Input;
            arParams[4].Value = specialGuid1.ToString();

            arParams[5] = new FbParameter("@SpecialGuid2", FbDbType.Char, 36);
            arParams[5].Direction = ParameterDirection.Input;
            arParams[5].Value = specialGuid2.ToString();

            arParams[6] = new FbParameter("@FromAddress", FbDbType.VarChar, 100);
            arParams[6].Direction = ParameterDirection.Input;
            arParams[6].Value = fromAddress;

            arParams[7] = new FbParameter("@ReplyTo", FbDbType.VarChar, 100);
            arParams[7].Direction = ParameterDirection.Input;
            arParams[7].Value = replyTo;

            arParams[8] = new FbParameter("@ToAddress", FbDbType.VarChar, 255);
            arParams[8].Direction = ParameterDirection.Input;
            arParams[8].Value = toAddress;

            arParams[9] = new FbParameter("@CcAddress", FbDbType.VarChar, 255);
            arParams[9].Direction = ParameterDirection.Input;
            arParams[9].Value = ccAddress;

            arParams[10] = new FbParameter("@BccAddress", FbDbType.VarChar, 255);
            arParams[10].Direction = ParameterDirection.Input;
            arParams[10].Value = bccAddress;

            arParams[11] = new FbParameter("@Subject", FbDbType.VarChar, 255);
            arParams[11].Direction = ParameterDirection.Input;
            arParams[11].Value = subject;

            arParams[12] = new FbParameter("@TextBody", FbDbType.VarChar);
            arParams[12].Direction = ParameterDirection.Input;
            arParams[12].Value = textBody;

            arParams[13] = new FbParameter("@HtmlBody", FbDbType.VarChar);
            arParams[13].Direction = ParameterDirection.Input;
            arParams[13].Value = htmlBody;

            arParams[14] = new FbParameter("@Type", FbDbType.VarChar, 50);
            arParams[14].Direction = ParameterDirection.Input;
            arParams[14].Value = type;

            arParams[15] = new FbParameter("@DateToSend", FbDbType.TimeStamp);
            arParams[15].Direction = ParameterDirection.Input;
            arParams[15].Value = dateToSend;

            arParams[16] = new FbParameter("@CreatedUtc", FbDbType.TimeStamp);
            arParams[16].Direction = ParameterDirection.Input;
            arParams[16].Value = createdUtc;


            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("INSERT INTO mp_EmailSendQueue (");
            sqlCommand.Append("Guid, ");
            sqlCommand.Append("SiteGuid, ");
            sqlCommand.Append("ModuleGuid, ");
            sqlCommand.Append("UserGuid, ");
            sqlCommand.Append("SpecialGuid1, ");
            sqlCommand.Append("SpecialGuid2, ");
            sqlCommand.Append("FromAddress, ");
            sqlCommand.Append("ReplyTo, ");
            sqlCommand.Append("ToAddress, ");
            sqlCommand.Append("CcAddress, ");
            sqlCommand.Append("BccAddress, ");
            sqlCommand.Append("Subject, ");
            sqlCommand.Append("TextBody, ");
            sqlCommand.Append("HtmlBody, ");
            sqlCommand.Append("Type, ");
            sqlCommand.Append("DateToSend, ");
            sqlCommand.Append("CreatedUtc )");

            sqlCommand.Append(" VALUES (");
            sqlCommand.Append("@Guid, ");
            sqlCommand.Append("@SiteGuid, ");
            sqlCommand.Append("@ModuleGuid, ");
            sqlCommand.Append("@UserGuid, ");
            sqlCommand.Append("@SpecialGuid1, ");
            sqlCommand.Append("@SpecialGuid2, ");
            sqlCommand.Append("@FromAddress, ");
            sqlCommand.Append("@ReplyTo, ");
            sqlCommand.Append("@ToAddress, ");
            sqlCommand.Append("@CcAddress, ");
            sqlCommand.Append("@BccAddress, ");
            sqlCommand.Append("@Subject, ");
            sqlCommand.Append("@TextBody, ");
            sqlCommand.Append("@HtmlBody, ");
            sqlCommand.Append("@Type, ");
            sqlCommand.Append("@DateToSend, ");
            sqlCommand.Append("@CreatedUtc )");
            sqlCommand.Append(";");

            int rowsAffected = FBSqlHelper.ExecuteNonQuery(
                GetWriteConnectionString(),
                sqlCommand.ToString(),
                arParams);

            return rowsAffected;

        }

        /// <summary>
        /// Deletes a row from the mp_EmailSendQueue table. Returns true if row deleted.
        /// </summary>
        /// <param name="guid"> guid </param>
        /// <returns>bool</returns>
        public static bool Delete(Guid guid)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_EmailSendQueue ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("Guid = @Guid ");
            sqlCommand.Append(";");
            FbParameter[] arParams = new FbParameter[1];

            arParams[0] = new FbParameter("@Guid", FbDbType.Char, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = guid.ToString();


            int rowsAffected = FBSqlHelper.ExecuteNonQuery(
                GetWriteConnectionString(),
                sqlCommand.ToString(),
                arParams);

            return (rowsAffected > -1);
        }

        /// <summary>
        /// Gets an IDataReader with rows from the mp_EmailSendQueue table where DateToSend >= CurrentTime.
        /// </summary>
        /// <param name="currentTime"> currentTime </param>
        public static IDataReader GetEmailToSend(DateTime currentTime)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  * ");
            sqlCommand.Append("FROM	mp_EmailSendQueue ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("DateToSend >= @CurrentTime ");
            sqlCommand.Append(";");

            FbParameter[] arParams = new FbParameter[1];

            arParams[0] = new FbParameter("@CurrentTime", FbDbType.TimeStamp);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = currentTime;

            return FBSqlHelper.ExecuteReader(
                GetReadConnectionString(),
                sqlCommand.ToString(),
                arParams);
        }

    }
}
