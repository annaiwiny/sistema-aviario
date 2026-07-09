from flask import Flask, request, jsonify, render_template_string
from datetime import datetime

app = Flask(__name__)

leituras = []

HTML = """
<!DOCTYPE html>
<html>
<head>
    <title>Monitoramento do Aviário</title>
    <meta http-equiv="refresh" content="2">
    <style>
        body{
            font-family: Arial;
            background:#f5f5f5;
            margin:40px;
        }
        table{
            border-collapse:collapse;
            width:100%;
            background:white;
        }
        th,td{
            border:1px solid #ccc;
            padding:10px;
            text-align:center;
        }
        th{
            background:#1976d2;
            color:white;
        }
        h1{
            color:#333;
        }
    </style>
</head>
<body>

<h1>Monitoramento do Aviário</h1>

<table>
<tr>
    <th>Data/Hora</th>
    <th>MAC</th>
    <th>Temperatura</th>
    <th>Umidade</th>
    <th>Nível de Água</th>
</tr>

{% for l in leituras %}
<tr>
    <td>{{l.data}}</td>
    <td>{{l.mac}}</td>
    <td>{{l.temp}} °C</td>
    <td>{{l.umidade}} %</td>
    <td>{{l.agua}}</td>
</tr>
{% endfor %}

</table>

</body>
</html>
"""

@app.route("/")
def index():
    return render_template_string(HTML, leituras=reversed(leituras))


@app.route("/api/sensors/readings", methods=["POST"])
def receber():

    dados = request.get_json()

    temperatura = None
    umidade = None
    agua = None

    for leitura in dados["readings"]:
        if leitura["type"] == 1:
            temperatura = leitura["value"]
        elif leitura["type"] == 2:
            umidade = leitura["value"]
        elif leitura["type"] == 3:
            agua = leitura["value"]

    leituras.append({
        "data": datetime.now().strftime("%d/%m/%Y %H:%M:%S"),
        "mac": dados["macAddress"],
        "temp": temperatura,
        "umidade": umidade,
        "agua": agua
    })

    return jsonify({
        "success": True,
        "message": "Leituras recebidas"
    }), 201


app.run(host="0.0.0.0", port=5000, debug=True)
