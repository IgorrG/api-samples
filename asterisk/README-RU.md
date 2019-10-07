# Использование сервера распознавания с Asterisk

### Устанавливаем зависимости

 - asterisk
 - docker
 - пакеты для работы со звуком: 

        sudo apt install sox espeak

 - пакет для python AGI и вебсокетов

        sudo pip3 install pyst2 websocket websocket-client

### Запускаем сервер распознавания

docker run -d -p 2700:2700 alphacep/kaldi-ru:latest

### Проверяем, что можем запустить EAGI скрипт

```
python3 eagi-ru.py
ARGS: ['eagi-ru.py']
^C
```

### Настраиваем план звонков

В etc/extensions.conf

```
exten => 200,1,Answer()
same = n,EAGI(/home/user/api-samples/asterisk/eagi-ru.py)
same = n,Hangup()
```

### Звоним и проверяем работу

