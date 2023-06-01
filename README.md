# Simulazione di moto orbitale in Unity

### Architettura
Uno dei principi più importanti di design Object-Oriented afferma che è meglio _favorire la composizione rispetto all’ereditarietà_. Questa affermazione ha come focus primario la manutenibilità del software e implicitamente si riferisce ad un altro importante principio che esorta a _programmare verso le interfacce e non verso l’implementazione_. Se l’obiettivo come sviluppatori è quello di raggiungere un alto disaccoppiamento ed una alta riusabilità del codice l’applicazione di questi due principi ci conduce sulla retta via.

Unity architetturalmente abbraccia questa filosofia alla radice implementando una **architettura a componenti**.

Lo sviluppo orientato ai componenti si preoccupa di definire le interfacce a cui i componenti devono conformarsi in modo che i client debbano solo preoccuparsi di quali interfacce necessitano e non di come sono implementate internamente. L’obiettivo dello sviluppatore in Unity è quello di scomporre il dominio del problema in componenti il più possibile autonomi per poi aggregare questi componenti in _contenitori_ rappresentanti le astrazioni importanti del dominio del problema. È importante soprattutto individuare _componenti comportamentali_ da implementare come classi riutilizzabili. In questo senso, pensare ad un design pattern come _Strategy_ può aiutare molto.

In Unity i componenti gestiti dal motore interno devono derivare da una classe base specifica chiamata **MonoBehaviour** e questo permette al motore di poter effettuare alcune _callback_ ridefinite in ogni componenete in alcuni momenti ben precisi. Naturalmente è possibile anche definire dei propri componenti come classi autonome che possono essere utilizzati tramite opportune interfacce come si vedrà nella simulazione.

Tra i componenti nativi che interessano maggiormenete questa simulazione sicuramente è importante citare il **Rigidbody** direttamente coinvolto dal modulo di fisica interno e tra le callback **FixedUpdate()** che permette di avere un _refresh rate_ della fisica disaccoppiato dal _frame rate_ grafico.

### Fisica

Questa semplice simulazione del moto orbitale dei pianeti viene eseguita usando solo il motore del modulo _built-in_ di fisica di Unity. Non viene eseguito nessun calcolo della traiettoria orbitale ma la traiettoria è il risultato dell'applicazione di forze e velocità ai corpi rigidi considerati. Si considera solo il moto di rivoluzione e non quello di rotazione.

In questa simulazione vengono usate solo due equazioni della meccanica classica.

La prima è la **legge di gravitazione universale di Newton**:

![prova](https://www.dl.dropboxusercontent.com/s/hpzy51cnavziyld/f1.jpg?dl=1)

Questa legge afferma che la forza gravitazionale **F** generata tra due corpi nello spazio è direttamente proporzionale al prodotto delle masse dei corpi e inversamente proporzionale al quadrato della distanza tra i centri di massa dei corpi. **G** è la costante di gravitazione universale e vale 6.674×10−11 N m^2 kg^−2 ma per esigenze di simulazione viene portata qui a circa **100** N m^2 kg^−2, **m1** e **m2** sono le masse dei corpi e **d^2** è la distanza al quadrato tra i corpi.

In Unity viene assegnata questa forza ai corpi rigidi e sarà poi compito del modulo di fisica calcolare l'accelerazione gravitazionale per ogni corpo come segue:

![prova2](https://www.dl.dropboxusercontent.com/s/n3ktsmyyazexm2h/f2.jpg?dl=1)


[![Watch the video](https://img.youtube.com/vi/<VIDEO_ID>/hqdefault.jpg)](https://www.dl.dropboxusercontent.com/s/tbsu4gn1d56g75q/20230519131325.mp4?dl=0 "Prova video")











