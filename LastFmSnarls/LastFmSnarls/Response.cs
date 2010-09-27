using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace LastFmSnarls
{
    public class Response
    {
        public string Content { get; private set; }
        public bool Success { get; private set; }
        public string Status {get; private set; }
        public Int32 ErrorCode { get; private set; }
        public string ErrorText { get; private set; }

        public List<Track> Tracks { get; private set; }
        public Track NowPlaying { get; private set; }
        public Track LastPlayed { get; private set; }

        public Response(HttpWebResponse response) {
            System.Globalization.CultureInfo enUS = new System.Globalization.CultureInfo("en-US");
            Tracks = new List<Track>();
            for (int i = 0; i < response.Headers.Count; i++)
            {
                KeyValuePair<string, string> header = new KeyValuePair<string, string>(response.Headers.GetKey(i), response.Headers.Get(i));
                switch (header.Key)
                {
                    case "Status":
                        this.Status = header.Value;
                        if(header.Value.StartsWith("200")) {
                            this.Success = true;
                        }
                        else
                        {
                            this.Success = false;
                        }
                        break;


                    default:
                        // any other response we are not interested...
                        break;
                }
            }


            XmlDocument xmlDoc = new XmlDocument();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();
                response.Close();
                xmlDoc.LoadXml(result);
            }

            XmlNodeList rootList = xmlDoc.GetElementsByTagName("lfm");
            XmlNode rootElement = rootList[0];
            if (true)
            {
                if (rootElement.Attributes["status"].Value.ToLower() == "ok")
                {
                    ErrorCode = 0;
                    Success = true;
                    ErrorText = "";
                    XmlNodeList allTracks = xmlDoc.GetElementsByTagName("track");
                    foreach (XmlNode currentNode in allTracks)
                    {
                        Track track = new Track();
                        try {
                            if(currentNode.Attributes["nowplaying"].Value == "true") {
                                track.NowPlaying = true;
                            }
                        } catch {}
                        foreach(XmlNode subNode in currentNode) {
                            switch(subNode.Name) {
                                case "artist":
                                    track.Artist = subNode.InnerText;
                                    break;

                                case "name":
                                    track.Name = subNode.InnerText;
                                    break;

                                case "album":
                                    track.Album = subNode.InnerText;
                                    break;

                                case "url":
                                    track.Link = subNode.InnerText;
                                    break;

                                case "image":
                                switch (subNode.Attributes["size"].Value) {
                                    case "small":
                                        track.AlbumArtSmall = subNode.InnerText;
                                        break;

                                    case "medium":
                                        track.AlbumArtMedium = subNode.InnerText;
                                        break;

                                    case "large":
                                        track.AlbumArtLarge = subNode.InnerText;
                                        break;

                                    case "extralarge":
                                        track.AlbumArtExtraLarge = subNode.InnerText;
                                        break;
                                }
                                break;

                                default:
                                    // ignoring
                                break;
                            }
                        
                        }
                        if (track.NowPlaying)
                        {
                            NowPlaying = track;
                        }
                        else
                        {
                            if (LastPlayed == null)
                            {
                                LastPlayed = track;
                            }
                        }
                        Tracks.Add(track);
                    }

                }
                else
                {
                    XmlElement errorElement = xmlDoc.GetElementById("error");
                    ErrorCode = Convert.ToInt32(errorElement.Attributes["code"].Value);
                    ErrorText = errorElement.Value;
                    Success = false;

                }
            }
            else
            {
                Success = false;
                ErrorCode = 999;
                ErrorText = "Invalid response from last.fm";
            }


        }


    }
}
