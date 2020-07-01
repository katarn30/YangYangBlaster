package internal

import (
	"github.com/azumads/GoogleIdTokenVerifier"
	"github.com/name5566/leaf/log"
)

var aud = "436238547875-ao5qlaml57b66s8l0s2l5i7lftoa6dc7.apps.googleusercontent.com"

func CallGoogleVerifyOauth2Token(idToken string) (string, bool) {

	bytes := GoogleIdTokenVerifier.GetCertsFromURL()
	certs := GoogleIdTokenVerifier.GetCerts(bytes)

	tokenInfo := GoogleIdTokenVerifier.VerifyGoogleIDToken(idToken, certs, aud)
	if tokenInfo == nil {
		return "", false
	}

	log.Debug("%v", tokenInfo)

	return tokenInfo.Sub, true
}
