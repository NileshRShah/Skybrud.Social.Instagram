using Skybrud.Social.Http;
using Skybrud.Social.Instagram.Objects;

namespace Skybrud.Social.Instagram.Exceptions {
    
    /// <summary>
    /// Class representing an exception/error returned by the Instagram API.
    /// </summary>
    /// <see>
    ///     <cref>https://www.instagram.com/developer/limits/</cref>
    /// </see>
    public class InstagramOAuthPermissionsException : InstagramOAuthException {

        /// <summary>
        /// Initializes a new exception based on the specified <paramref name="response"/> and <paramref name="meta"/> data.
        /// </summary>
        /// <param name="response">The response the exception should be based on.</param>
        /// <param name="meta">The meta data with information about the exception.</param>
        public InstagramOAuthPermissionsException(SocialHttpResponse response, InstagramMetaData meta) : base(response, meta) { }

    }

}