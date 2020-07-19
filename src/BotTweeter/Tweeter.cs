﻿using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace BotTweeter
{
    public class Tweeter
    {
        private readonly TwitterContext _context;

        public readonly String API_KEY = "lcwwbboVACDsHPOkjkantPEQO";
        public readonly String API_SECRET_KEY = "g2mzLY5koijxhAXPdBZCYMUme3zSUZ8Jhx5vtFu66AcNFv1uph";
        public readonly String ACCESS_TOKEN = "1158127961108881408-C1zk9cVv5AbArrXcCStIIKMSn3i1EI";
        public readonly String ACCESS_TOKEN_SECRET = "zXqcQCSxvQ6fTTevzfdrGLi19Op35GXjimXkW1IMRXDPD";

        // public readonly String AVASARALA_ID = "@AvasaralaBot";
        public readonly String AVASARALA_ID = "@JamieTheDyer";
        public readonly String AVASARALA_NAME = @"""Chrisjen Avasarala""";
        // public readonly String AVASARALA_TAG = "#WhatWouldAvasaralaSay";
        // public readonly String AVASARALA_TAG = "#whatwouldavasaralado";
        public readonly String AVASARALA_TAG = "#caturday";

        public Tweeter()
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = API_KEY,
                    ConsumerSecret = API_SECRET_KEY,
                    AccessToken = ACCESS_TOKEN,
                    AccessTokenSecret = ACCESS_TOKEN_SECRET
                }
            };

            _context = new TwitterContext(auth);
        }

        public void Menu()
        {
            Int32 numberOfTweets = 20;
            Boolean keyValueSuccess = false;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.DarkBlue;

            while (true)
            {
                Console.Clear();

                Console.WriteLine("Wisdom of Avasarala");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine($"1: Check {AVASARALA_ID} mentions");
                Console.WriteLine($"2: Search for {AVASARALA_NAME}");
                Console.WriteLine($"3: Search for {AVASARALA_TAG}");
                Console.WriteLine($"4: Get last {numberOfTweets} tweets from {AVASARALA_ID}");
                Console.WriteLine("5: Test tweet some wisdom (not implemented)");
                Console.WriteLine("6: Tweet some wisdom (not implemented)");
                Console.WriteLine();
                Console.WriteLine("<ESC>: Quit");

                ConsoleKeyInfo keyInfo = Console.ReadKey(false);
                if (keyInfo.Key == ConsoleKey.Escape)
                    return;

                var keyChar = keyInfo.KeyChar;
                Int32 keyValue;
                keyValueSuccess = Int32.TryParse(keyChar.ToString(), out keyValue);

                if (keyValue == 1)
                {
                    GetTweets(AVASARALA_ID);
                    PressAKey();
                }
                else if (keyValue == 2)
                {
                    GetTweets(AVASARALA_NAME);
                    PressAKey();
                }
                else if (keyValue == 3)
                {
                    GetTweets(AVASARALA_TAG);
                    PressAKey();
                }
                else if (keyValue == 4)
                {
                    GetTweetsFrom(AVASARALA_ID, numberOfTweets);
                    PressAKey();
                }
                else if (keyValue == 5)
                {
                    var tweetText = "";
                    MaybeTweet(tweetText, false);
                    PressAKey();
                }
                else if (keyValue == 6)
                {
                    var tweetText = "";
                    MaybeTweet(tweetText, true);
                    PressAKey();
                }
            }
        }

        public void GetTweets(String searchText, Int32 numberOfTweets=100)
        {
            var searchResponse =
                (from search in _context.Search
                 where search.Type == SearchType.Search &&
                       search.Query == searchText &&
                       search.SearchLanguage == "en" &&
                       search.TweetMode == TweetMode.Extended &&
                    //    search.Until == new DateTime(2018, 12, 31) &&
                       search.Count == numberOfTweets
                 select search).SingleOrDefault();

            foreach (var tweet in searchResponse.Statuses)
            {
                if (tweet.Retweeted)
                    continue;

                Console.WriteLine();
                Console.WriteLine($"From: {tweet.User.ScreenNameResponse}, Likes: {tweet.FavoriteCount}, Retweets: {tweet.RetweetCount}, Date: {tweet.CreatedAt}");
                Console.WriteLine($"{tweet.FullText}");
                Console.WriteLine();
            }
        }

        public void GetTweetsFrom(String tweeter, Int32 numberOfTweets=100)
        {
            var tweetList =
                (from search in _context.Status
                 where search.Type == StatusType.User &&
                       search.ScreenName == tweeter &&
                       search.TweetMode == TweetMode.Extended &&
                       search.Count == numberOfTweets
                 select search).ToList();

            foreach (Status tweet in tweetList)
            {
                Console.WriteLine();
                Console.WriteLine($"From: {tweet.User.ScreenNameResponse}, Likes: {tweet.FavoriteCount}, Retweets: {tweet.RetweetCount}, Date: {tweet.CreatedAt}");
                Console.WriteLine($"{tweet.FullText}");
                Console.WriteLine();
            }
        }

        public void MaybeTweet(String tweetText, Boolean actuallyTweet)
        {
            if (actuallyTweet)
            {
                Tweet(tweetText, null);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Not actually tweeted:");
                Console.WriteLine(tweetText);
            }
        }


        public void Tweet(String tweetText, List<UInt64> mediaIds)
        {
            try
            {
                Status status;
                if (mediaIds.Count == 0)
                    status = _context.TweetAsync(tweetText).Result;
                else
                    status = _context.TweetAsync(tweetText, mediaIds).Result;

                if (status == null)
                {
                    Console.WriteLine("Tweet failed to process, but API did not report an error");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void PressAKey()
        {
            Console.WriteLine();
            Console.WriteLine("Press a key to continue.");

            Console.ReadKey(false);
        }
    }
}