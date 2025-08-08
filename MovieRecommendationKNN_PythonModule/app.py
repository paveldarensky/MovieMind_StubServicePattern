# app.py
from flask import Flask, request, jsonify
from model import recommend_movies  # Импортируем функцию из model.py

app = Flask(__name__)

@app.route('/recommend', methods=['POST'])
def recommend_api():
    data = request.get_json()

    try:
        year = int(data['year'])
        duration = int(data['duration'])
        rating = float(data['rating'])
        genres = data['genres']  # список жанров

        recommended_movies, distances = recommend_movies(year, rating, duration, genres)

        result = [{"title": title, "distance": float(dist)} for title, dist in zip(recommended_movies, distances)]
        return jsonify(result)

    except Exception as e:
        return jsonify({'error': str(e)}), 400

if __name__ == '__main__':
    app.run(port=5000, debug=True)