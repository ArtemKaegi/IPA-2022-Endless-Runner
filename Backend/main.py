import logging
from flask import Flask
import mysql.connector
import json

app = Flask(__name__)

logging.basicConfig(level=logging.INFO, filename="backen.log", format='%(asctime)s %(levelname)-8s %(message)s')

try:
    mydb = mysql.connector.connect(
        host="localhost",
        user="gameBackend",
        password="Artom125332?",
        database="endless_runner_backend"
    )

    logging.info("Connection Succesful")
except mysql.connector.Error as err:
    logging.error("Something went wrong: {}".format(err))


@app.route("/createNewPlayer/<playerName>/<deviceId>")
def create_new_player(playerName, deviceId):
    mycursor = mydb.cursor()
    try:
        mycursor.execute("INSERT INTO players(playerName, deviceId) VALUES('" + playerName + "', '" + deviceId + "')")
        mycursor.close()
        mydb.commit()
        logging.info("Successfully ran: INSERT INTO players(playerName, deviceId) VALUES('" + playerName + "', '" + deviceId + "')")
        return "Success"
    except mysql.connector.Error as err:
        logging.error("Something went wrong: {}".format(err))
        return "Failure"


@app.route("/setNewHighscore/<deviceId>/<highscore>")
def create_new_highscore(deviceId, highscore):
    try:
        mycursor = mydb.cursor()
        mycursor.execute("SELECT playerId FROM players WHERE deviceId = '" + deviceId + "'")
        x = mycursor.fetchall()
        print("INSERT INTO highscores(playerId, highscore) VALUES(" + str(x[0][0]) + ", " + highscore + ")")
        mycursor.execute("INSERT INTO highscores(playerId, highscore) VALUES(" + str(x[0][0]) + ", " + highscore + ")")
        mycursor.close()
        mydb.commit()
        logging.info("Successfully ran: INSERT INTO highscores(playerId, highscore) VALUES(" + str(x[0][0]) + ", " + highscore + ")")
        return str(x)
    except mysql.connector.Error as err:
        logging.error("Something went wrong: {}".format(err))
        return "Failure"


@app.route("/getHighscores")
def get_highscores():
    try:
        mycursor = mydb.cursor()
        mycursor.execute("SELECT * FROM highscores")
        mycursor.close()
        logging.info("Successfully ran: SELECT * FROM highscores")
        return str(mycursor.fetchall())
    except mysql.connector.Error as err:
        logging.error("Something went wrong: {}".format(err))
        return "Failure"


app.run(host="0.0.0.0")

# print(mycursor.fetchall())
