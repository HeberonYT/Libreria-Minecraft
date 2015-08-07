
Namespace Auth

	Public Class AuthResponse
		Private accessToken As String = Nothing
		Private clientToken As String = Nothing
		Private username As String = Nothing
		Private userID As String = Nothing
		Private profileID As String = Nothing
		Private twitchToken As String = Nothing
		Private [error] As [Boolean] = True
		Public Sub New(accessToken As String, clientToken As String, username As String, userID As String, profileID As String, twitchToken As String, _
			[error] As [Boolean])
			Me.accessToken = accessToken
			Me.clientToken = clientToken
			Me.username = username
			Me.userID = userID
			Me.profileID = profileID
			Me.twitchToken = twitchToken
			Me.[error] = [error]
		End Sub
		Public Sub New([error] As [Boolean])
			Me.[error] = [error]
		End Sub
		Public Function getAccessToken() As String
			Return Me.accessToken
		End Function
		Public Function getClientToken() As String
			Return Me.clientToken
		End Function
		Public Function getUsername() As String
			Return Me.username
		End Function
		Public Function getUserID() As String
			Return Me.userID
		End Function
		Public Function getProfileID() As String
			Return Me.profileID
		End Function
		Public Function getTwitchToken() As String
			Return Me.twitchToken
		End Function
		Public Function hasError() As [Boolean]
			Return Me.[error]
		End Function
		Public Function toAuthData() As AuthData
			Return New AuthData(Me)
		End Function
	End Class
End Namespace