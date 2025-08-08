# Подключение необходимых библиотек и загрузка данных
import numpy as np
import pandas as pd
from sklearn.preprocessing import MultiLabelBinarizer, StandardScaler
from sklearn.neighbors import NearestNeighbors

###################################### Для модели ######################################

# загрузка данных
file_path = r"C:\Users\pavel\Programming\TSU\SEMESTER_6\OOAD\LW4\LW4\Resources\IMBD.csv"
data = pd.read_csv(file_path, sep=',')

# выведем 10 случайно выбранных образцов из набора данных
# print(data.sample(10))

# Выведем информацию о наборе данных
# data.info()

# Подсчитать количество пропущенных значений в каждом поле также можно с помощью следующей функции
# missing_value_cnt = data.isnull().sum()
# print(missing_value_cnt)

# Для дальнейшей работы признак certificate заполним модой, признаки duration (значение будет в минутах, целое), rating, votes (будет целое) заполним средним значением

# 1. Признак 'certificate' (категориальный) заполняем самым частым значением (модой)
data['certificate'] = data['certificate'].fillna(data['certificate'].mode()[0])

# 2. Признак 'duration' (строка вида "85 min") нужно сначала очистить и привести к числу
#    Удаляем 'min' и преобразуем к типу int
data['duration'] = data['duration'].str.replace(' min', '')
data['duration'] = pd.to_numeric(data['duration'], errors='coerce')  # Преобразуем с заменой некорректных значений на NaN

# Теперь можно заполнить пропуски в 'duration' средним значением (округляем до целого)
data['duration'] = data['duration'].fillna(round(data['duration'].mean()))
data['duration'] = data['duration'].astype(int)

# 3. Признак 'rating' (числовой) заполняем средним значением
data['rating'] = data['rating'].fillna(data['rating'].mean())

# 4. Признак 'votes' (строка с запятыми, например "4,202") нужно сначала убрать запятые, потом привести к числу
data['votes'] = data['votes'].str.replace(',', '')
data['votes'] = pd.to_numeric(data['votes'], errors='coerce')

# Заполняем пропуски в 'votes' средним значением (округляем до целого)
data['votes'] = data['votes'].fillna(round(data['votes'].mean()))
data['votes'] = data['votes'].astype(int)

# Теперь данные готовы для анализа
# проверим информацию о наборе данных после предобработки
# data.info()

# посмотрим что стало с пропусками
# missing_value_cnt = data.isnull().sum()
# print(missing_value_cnt)

# Создаем копию датасета
data_year_cleaned = data.copy()

# Удаляем пропуски, оставляя только строки с непустым годом
data_year_cleaned = data_year_cleaned[~data_year_cleaned['year'].isna()].copy()

# Извлекаем первый год из строки (даже если их два через пробел)
data_year_cleaned['year'] = data_year_cleaned['year'].str.extract(r'(\d{4})').astype(float)

# Удалим строки с отсутствующим годом
data_year_cleaned = data_year_cleaned.dropna(subset=['year'])

# Преобразуем к int, теперь безопасно
data_year_cleaned['year'] = data_year_cleaned['year'].astype(int)

######################################

# Создаем категорию 'old' или 'new' в зависимости от года
data_year_cleaned['category'] = data_year_cleaned['year'].apply(lambda x: 'old' if x < 2000 else 'new')

# Разделяем жанры на отдельные значения и приводим к DataFrame
data_year_cleaned['genre'] = data_year_cleaned['genre'].apply(lambda x: x.split(', ') if isinstance(x, str) else [])

# Создаем DataFrame для каждого жанра
genre_data_year_cleaned = data_year_cleaned.explode('genre')

# genre_data_year_cleaned.info()
# print(genre_data_year_cleaned['genre'].unique())

# Группируем по жанру и году, считаем среднее по интересующим параметрам
agg_by_year_genre = genre_data_year_cleaned.groupby(['genre', 'year']).agg({
    'rating': 'mean',
    'duration': 'mean',
    'votes': 'mean',
    'title': 'count'  # сколько фильмов в этом жанре в этом году
}).rename(columns={'title': 'count'}).reset_index()

# print(agg_by_year_genre.sample(10))




########################################### Выбор модели и подготовка признаков ###########################################
df = data_year_cleaned.copy()
# df.info()

# print(df['genre'].sample(5))

# print(df.describe(include='all'))

# Оставляем нужные колонки
df = df[['title', 'year', 'duration', 'rating', 'genre']]

# Удалим выбросы по duration и year (грубо)
df = df[(df['duration'] >= 30) & (df['duration'] <= 240)]
df = df[(df['year'] >= 1930) & (df['year'] <= 2025)]

# Удаляем строки с NaN в колонке 'genre'
df_cleaned = df.dropna(subset=['genre'])

mlb = MultiLabelBinarizer()
genre_encoded = mlb.fit_transform(df['genre'])

# Переводим в DataFrame и даём названия столбцам
genre_df = pd.DataFrame(genre_encoded, columns=mlb.classes_, index=df.index)

scaler = StandardScaler()
scaled_features = scaler.fit_transform(df[['year', 'duration', 'rating']])
scaled_df = pd.DataFrame(scaled_features, columns=['year', 'duration', 'rating'], index=df.index)

# Объединяем числовые и категориальные признаки
features_df = pd.concat([scaled_df, genre_df], axis=1)

titles = df['title'].reset_index(drop=True)


# Инициализация модели KNN
knn = NearestNeighbors(n_neighbors=5, metric='cosine')  # Можно настроить параметр n_neighbors

# Обучаем модель на подготовленных данных
knn.fit(features_df)



def recommend_movies(year, rating, duration, genres):
    # Создаем список признаков на основе введенных параметров
    # Стандартизируем числовые параметры (год, длительность, рейтинг)
    scaled_df = pd.DataFrame([[year, duration, rating]], columns=['year', 'duration', 'rating'])
    scaled_params = scaler.transform(scaled_df)[0]

    # Преобразуем жанры в one-hot формат
    genre_params = np.zeros(len(mlb.classes_))  # Массив для жанров
    genre_indices = [mlb.classes_.tolist().index(genre) for genre in genres if genre in mlb.classes_]
    genre_params[genre_indices] = 1  # Устанавливаем 1 в местах соответствующих жанров

    # Собираем все параметры в один массив
    movie_params = np.concatenate([scaled_params, genre_params])

    # Находим ближайшие фильмы
    movie_df = pd.DataFrame([movie_params], columns=features_df.columns)
    distances, indices = knn.kneighbors(movie_df, n_neighbors=50)

    # Получаем заголовки ближайших фильмов
    nearest_movies = titles.iloc[indices[0]]

    return nearest_movies, distances[0]




# # Пример запроса: жанры - Action, Adventure; год - 2020; рейтинг - 8.5; длительность - 120 минут
# recommended_movies, distances = recommend_movies(year=2020, rating=8.5, duration=120, genres=['Action', 'Adventure'])
#
# # Выводим результаты
# for movie, dist in zip(recommended_movies, distances):
#     print(f"Фильм: {movie}, Расстояние: {dist}")