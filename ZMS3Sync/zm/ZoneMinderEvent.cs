using System;
using System.IO;
using MySql.Data.MySqlClient;

namespace ZMS3Sync
{
    public class ZoneMinderEvent
    {





        public ZoneMinderEvent(long frameId, string monitorname)
        {
            this.frameID = frameId;
            this.monitorName = monitorname;
            
        }

        public long frameID { get; set; }
        public string monitorName { get; set; }
        public DateTime starttime { get; set; }



        public static void getEvents()
        {

            U.log("Starting main event loop");

            var upl = new S3Uploader(U.config["S3Region"], U.config["S3Bucket"]);

            var basePath = U.config["ImgBasePath"];
            if (basePath.Substring(basePath.Length - 1) != "/")
            {
                basePath += "/";
                
            }

            var sleepSecs = U.config["LoopPauseSeconds"] ?? "3";



            var sleepMs = Convert.ToInt32(sleepSecs) * 1000;


            using (MySqlConnection connReader = new MySqlConnection(U.config["MySqlConnection"]))
            using (MySqlConnection connWriter = new MySqlConnection(U.config["MySqlConnection"]))
            {

             
                while (true)
                {
                    if(connReader.State!=System.Data.ConnectionState.Open)
                    {
                        connReader.Open();
                    }

                    if (connWriter.State != System.Data.ConnectionState.Open)
                    {
                        connWriter.Open();
                    }


                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(U.config["ZMQuery"], connReader);
                        U.log("Checking for new events");
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                var e = new ZoneMinderEvent(Convert.ToInt64(reader["frameid"]), reader["monitor_name"].ToString());
                                e.starttime = Convert.ToDateTime(reader["starttime"]);
                                var img = new zmImage(e, U.config["ImgBasePath"], Convert.ToInt64(reader["eventid"]));
                                if (File.Exists(img.fpath))
                                {
                                    try
                                    {
                                        var s3Key = e.monitorName + "/" + e.starttime.ToString("s");
                                        //fire and forget here..
                                        upl.uploadFile(img.fpath, s3Key);
                                        var sql = $"insert alarm_uploaded (frameid,upload_timestamp,eventid) values ( {e.frameID}, CURRENT_TIMESTAMP, {img.eventId})";
                                        MySqlCommand insertCmd = new MySqlCommand(sql, connWriter);
                                        var rowsAffected = insertCmd.ExecuteNonQuery();
                                        if (rowsAffected <= 0)
                                        {

                                            U.log("Did not insert a row into alarm_uploaded");
                                        }
                                    }
                                    catch (Exception exc)
                                    {
                                        U.log("Error uploading/updating db", exc);
                                    }

                                }
                                else
                                {
                                    U.log("File does not exist - " + img.fpath);
                                }



                            }
                        }
                    }
                    catch(Exception e)
                    {
                        U.log("Error in outer loop",e);
                        
                    }
                    System.Threading.Thread.Sleep(sleepMs);
                }
            }
                //return list;  
            }  

        }
    }

