generowanei hasła mqtt
docker run -it --rm -v ./mqtt5/config:/mosquitto/config eclipse-mosquitto:alpine mosquitto_passwd -c /mosquitto/config/pwfile admin

powstałem plikowi nadać odpowiednie uprawnienia
sudo chmod 644 ./mqtt5/config/pwfile

pamiętać że hasło podane w komedzie ma się zgadzać z tym w .env

linux is stoopid
~/.dotnet/tools/dotnet-ef migrations add InitialCreate --project Infrastructure --startup-project Api