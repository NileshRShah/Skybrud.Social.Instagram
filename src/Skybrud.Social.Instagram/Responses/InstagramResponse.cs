﻿using System;
using System.Net;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.Http;
using Skybrud.Social.Instagram.Exceptions;
using Skybrud.Social.Instagram.Models;
using Skybrud.Social.Instagram.Models.Common;

namespace Skybrud.Social.Instagram.Responses {

    /// <summary>
    /// Class representing a response from the Instagram API.
    /// </summary>
    public class InstagramResponse : SocialResponse {

        #region Properties
        
        /// <summary>
        /// Gets information about rate limiting.
        /// </summary>
        public InstagramRateLimiting RateLimiting { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The underlying raw response the instance should be based on.</param>
        protected InstagramResponse(SocialHttpResponse response) : base(response) {
            RateLimiting = InstagramRateLimiting.GetFromResponse(response);
        }

        #endregion
        
        #region Static methods

        /// <summary>
        /// Validates the specified <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The response to be validated.</param>
        public static void ValidateResponse(SocialHttpResponse response) {

            // Skip error checking if the server responds with an OK status code
            if (response.StatusCode == HttpStatusCode.OK) return;

            // Handle errors when the response body isn't JSON
            if (!response.Body.StartsWith("{")) {
                switch (response.StatusCode) {
                    case HttpStatusCode.NotFound:
                        throw new InstagramNotFoundException(response);
                    default:
                        throw new InstagramHttpException(response);
                }
            }

            // Parse the response body
            JObject obj = ParseJsonObject(response.Body);

            // Get the "meta" object (may be the root object for some errors)
            InstagramMetaData meta = obj.HasValue("code") ? InstagramMetaData.Parse(obj) : obj.GetObject("meta", InstagramMetaData.Parse);

            // If the type isn't provided (it really should), we just throw an exception of the base type
            if (String.IsNullOrWhiteSpace(meta.ErrorType)) {
                throw new InstagramHttpException(response, meta);
            }
            
            // Handle OAuth exception types
            if (meta.ErrorType.StartsWith("OAuth")) {
                
                switch (meta.ErrorType) {

                    case "OAuthAccessTokenException":
                        throw new InstagramOAuthAccessTokenException(response, meta);

                    case "OAuthForbiddenException":
                        throw new InstagramOAuthForbiddenException(response, meta);

                    case "OAuthParameterException":
                        throw new InstagramOAuthParameterException(response, meta);

                    case "OAuthPermissionsException":
                        throw new InstagramOAuthPermissionsException(response, meta);

                    case "OAuthRateLimitException":
                        throw new InstagramOAuthRateLimitException(response, meta);

                    default:
                        throw new InstagramOAuthException(response, meta);

                }
            
            }

            // Handle other error types
            switch (meta.ErrorType) {
                
                case "APINotFoundError":
                    throw new InstagramNotFoundException(response, meta);
                
                default:
                    throw new InstagramHttpException(response, meta);
            
            }

        }

        #endregion

    }

    /// <summary>
    /// Class representing a response from the Instagram API.
    /// </summary>
    public class InstagramResponse<T> : InstagramResponse {

        #region Properties

        /// <summary>
        /// Gets the body of the response.
        /// </summary>
        public T Body { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The underlying raw response the instance should be based on.</param>
        protected InstagramResponse(SocialHttpResponse response) : base(response) { }

        #endregion

    }

}