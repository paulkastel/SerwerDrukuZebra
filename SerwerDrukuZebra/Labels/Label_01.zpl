^XA

^FX ==Komentarz: boksy do obramowania.==
^FO 25, 30^GB 380, 592, 3^FS
^FO 25, 670^GB 380, 592, 3^FS
^FO 25, 1300^GB 380, 370, 3^FS

^FX Parametry fontu i obrót tekstu o 90stop
^CF 0, 40
^FWR

^FX ==========Etykieta Magazyn=========
^FO350,200^FDMAGAZYN !_nazwamagazyn_! ^FS

^FX kod kreskowy-MAGAZYN
^FO100,100^BY3
^B3R,N,90,N,N
^FO240,50^FD !_kodkreskowy_! ^FS

^CF 0, 25
^FWR

^FX liniapierwsza
^FO 190, 45^FD  !_gatunekblachy_!  ^FS
^FO 190, 310^FD  !_typblachy_!  ^FS
^FO 190, 450^FD  !_szerokoscblachy_!  x  !_wysokoscblachy_!  ^FS

^FX liniadruga
^FO 145, 45^FD mkw.^FS
^FO 145, 390^FD ton ^FS
^FO 145, 450^FD  !_masa_!  ^FS

^FX liniatrzecia
^FO 100, 45^FD !_kodkreskowy_!^FS
^FO 100, 390^FD  !_datatime_!  ^FS

^CF 0, 30
^FWR
^FO 45, 45^FD  !_rodzajblachy_! ^FS
^CF 0, 40
^FWR

^FX =======Etykieta Produkcja==========
^FO350,865^FDPRODKCJA^FS

^FX kod kreskowy-PRODUKCJA
^FO100,100^BY3
^B3R,N,90,N,N
^FO240, 690^FD !_kodkreskowy_! ^FS

^CF 0, 25
^FWR
^FX liniapierwsza
^FO 190, 695^FD   !_gatunekblachy_!  ^FS
^FO 190, 960^FD  !_typblachy_! ^FS
^FO 190, 1100^FD  !_szerokoscblachy_!  x  !_wysokoscblachy_! ^FS

^ FX liniadruga
^FO 145, 695^FD mkw.^FS
^FO 145, 800^FD  !_metrkwadrat_! ^FS
^FO 145, 1040^FD ton ^FS
^FO 145, 1100^FD  !_masa_!  ^FS

^FX liniatrzecia
^FO 100, 695^FD !_kodkreskowy_! ^FS
^FO 100, 1040^FD  !_datatime_!  ^FS

^CF 0, 30
^FWR
^FO45, 695^FD !_rodzajblachy_! ^FS
^FO 45, 1150^FD  !_metrkwadrat_! ^FS

^CF0,30
^FX ======Trzecia etykieta BLACHA=======
^FX pierwsza linia
^FO 40, 1320^A0N^FD !_kodkreskowy_! ^FS
^FO 230, 1320^A0N^FD  !_datatime_!  ^FS

^FX druga linia
^FO 40, 1400^A0N^FD  !_typblachy_! ^FS
^FO 230, 1400^A0N^FD !_szerokoscblachy_!  x  !_wysokoscblachy_!  ^FS

^ FX trzecia linia
^FO 40, 1480^A0N^FD mkw.^FS
^FO 130, 1480^A0N^FD !_metrkwadrat_! ^FS
^FO 220, 1480^A0N^FD ton ^FS
^FO 290, 1480^A0N^FD  !_masa_!  ^FS

^FO 40, 1620^A0N^FD !_rodzajblachy_!  ^FS
^FO 300, 1620^A0N^FD !_metrkwadrat_!  ^FS

^XZ"