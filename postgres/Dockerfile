FROM postgres:latest
RUN localedef -i ru_RU -c -f UTF-8 -A /usr/share/locale/locale.alias ru_RU.UTF-8
ENV LANG ru_RU.utf8


RUN mkdir -p /docker-entrypoint-initdb.d
COPY 1.sh /docker-entrypoint-initdb.d
