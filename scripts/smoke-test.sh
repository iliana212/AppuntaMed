#!/usr/bin/env bash
# Smoke test of the Docker stack. It checks the things that only break when the pieces run together:
# the API is up, it rejects anonymous calls, Keycloak issues a token, and the API ACCEPTS that token
# (issuer and signing keys validated across containers) on top of a migrated, seeded database.
#
#   docker compose up -d --build --wait
#   bash scripts/smoke-test.sh                 # API_URL defaults to http://localhost:8081
#
# Needs: bash, curl and jq.
set -euo pipefail

API="${API_URL:-http://localhost:8081}"

fail() {
  echo "FAIL: $*" >&2
  exit 1
}

status_of() {
  curl -s -o /dev/null -w '%{http_code}' "$@"
}

echo "1/5 The API is healthy"
body="$(curl -fsS "$API/health")" || fail "GET /health did not answer"
[ "$body" = "Healthy" ] || fail "GET /health answered '$body'"

echo "2/5 An anonymous request is rejected"
code="$(status_of "$API/api/professions")"
[ "$code" = "401" ] || fail "GET /api/professions without a token answered $code (expected 401)"

echo "3/5 Keycloak issues a token for the demo patient"
token="$(curl -fsS -X POST "$API/api/auth" \
  -H 'Content-Type: application/json' \
  -d '{"user":"ana","psswd":"Patient123!"}' | jq -r '.access_token // empty')" || fail "login request failed"
[ -n "$token" ] || fail "the login did not return an access_token"

echo "4/5 The API accepts the token"
code="$(status_of -H "Authorization: Bearer $token" "$API/api/professions")"
[ "$code" = "200" ] || fail "GET /api/professions with a valid token answered $code (expected 200)"

echo "5/5 The catalog and the demo data are in the database"
professions="$(curl -fsS -H "Authorization: Bearer $token" "$API/api/professions" | jq 'length')"
doctors="$(curl -fsS -H "Authorization: Bearer $token" "$API/api/doctors" | jq 'length')"
[ "$professions" -ge 1 ] || fail "the profession catalog is empty"
[ "$doctors" -ge 2 ] || fail "the demo doctors were not seeded (found $doctors)"

echo "OK: the stack works end to end"
