namespace SkillSnap.Client.Services
{
    /// <summary>
    /// Service for managing user session state across components
    /// </summary>
    public class UserSessionService
    {
        private string? _userId;
        private string? _userName;
        private string? _email;
        private Dictionary<string, object> _sessionData = new();

        /// <summary>
        /// Event triggered when session state changes
        /// </summary>
        public event Action? OnChange;

        /// <summary>
        /// Gets or sets the current user ID
        /// </summary>
        public string? UserId
        {
            get => _userId;
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    NotifyStateChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current user name
        /// </summary>
        public string? UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    NotifyStateChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current user email
        /// </summary>
        public string? Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    NotifyStateChanged();
                }
            }
        }

        /// <summary>
        /// Checks if a user is logged in
        /// </summary>
        public bool IsAuthenticated => !string.IsNullOrEmpty(_userId);

        /// <summary>
        /// Set the current user information
        /// </summary>
        public void SetUser(string userId, string userName, string email)
        {
            _userId = userId;
            _userName = userName;
            _email = email;
            NotifyStateChanged();
        }

        /// <summary>
        /// Clear the current user session
        /// </summary>
        public void ClearUser()
        {
            _userId = null;
            _userName = null;
            _email = null;
            _sessionData.Clear();
            NotifyStateChanged();
        }

        /// <summary>
        /// Store arbitrary session data
        /// </summary>
        public void SetSessionData(string key, object value)
        {
            _sessionData[key] = value;
            NotifyStateChanged();
        }

        /// <summary>
        /// Retrieve session data
        /// </summary>
        public T? GetSessionData<T>(string key)
        {
            if (_sessionData.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }

        /// <summary>
        /// Remove session data by key
        /// </summary>
        public void RemoveSessionData(string key)
        {
            if (_sessionData.Remove(key))
            {
                NotifyStateChanged();
            }
        }

        /// <summary>
        /// Check if session data exists
        /// </summary>
        public bool HasSessionData(string key)
        {
            return _sessionData.ContainsKey(key);
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
