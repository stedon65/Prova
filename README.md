# Moto orbitale in Unity

### Introduzione
Uno dei principi più importanti del design orientato agli oggetti afferma che è meglio _favorire la composizione rispetto all’ereditarietà_. Questa affermazione ha come focus primario la manutenibilità del software e implicitamente si riferisce ad un altro importante principio che esorta a _programmare verso le interfacce e non verso l’implementazione_. Se l’obiettivo come sviluppatori è quello di raggiungere un alto disaccoppiamento ed un'alta riusabilità del codice l’applicazione di questi due principi ci conduce sulla retta via.

Unity architetturalmente abbraccia questa filosofia alla radice implementando una **architettura a componenti**.

Lo sviluppo orientato ai componenti si preoccupa di definire le interfacce a cui i componenti devono conformarsi in modo che i client degli oggetti debbano solo interessarsi di quali protocolli di comunicazione necessitano e non di come sono implementati internamente. L’obiettivo dello sviluppatore in Unity è quello di _scomporre il dominio del problema in componenti_ il più possibile autonomi per poi aggregare questi componenti in _contenitori_ rappresentanti le astrazioni importanti del dominio stesso. È importante soprattutto individuare _componenti comportamentali_ da implementare come classi riutilizzabili. In questo senso, pensare ad un design pattern come _Strategy_ può aiutare molto.

In Unity i componenti gestiti dal motore interno devono derivare da una classe base specifica chiamata **MonoBehaviour** e questo permette al motore di poter effettuare alcune _callback_ ridefinite in ogni componenete in alcuni momenti ben precisi. Naturalmente è possibile anche definire dei propri componenti come classi autonome che possono essere utilizzati tramite opportune interfacce come si vedrà nel codice della simulazione.

Tra i componenti nativi che interessano maggiormenete questa simulazione sicuramente è importante citare il **Rigidbody** direttamente coinvolto dal modulo di fisica interno e tra le callback **FixedUpdate** che permette di avere un _refresh rate_ della fisica costante disaccoppiato dal _frame rate_ grafico.

[Fisica](https://github.com/stedon65/moto-orbitale-in-unity/blob/main/Fisica.md)

[Codice](https://github.com/stedon65/moto-orbitale-in-unity/blob/main/Codice.md)

[Simulazioni](https://github.com/stedon65/moto-orbitale-in-unity/blob/main/Simulazioni.md)

### Fisica

![](https://dl.dropboxusercontent.com/s/ziekoxun8flpd8o/diagram3.png?dl=1)

Questa semplice simulazione del moto orbitale dei pianeti viene eseguita usando solo il motore del modulo di fisica di Unity. Non viene eseguito nessun calcolo della traiettoria orbitale da programma ma la traiettoria è il risultato dell'applicazione di forze e velocità ai corpi rigidi. **Si considera solo il moto di rivoluzione e non quello di rotazione**.

Vengono usate solo due equazioni della fisica classica.

La prima è la **legge di gravitazione universale di Newton**:

### $$F_G=G \dfrac{m_1m_2}{d^2}$$

Questa legge afferma che la forza gravitazionale $F_G$ generata tra due corpi nello spazio è direttamente proporzionale al prodotto delle masse dei corpi e inversamente proporzionale al quadrato della distanza tra i centri dei corpi. $G$ è la costante di gravitazione universale e vale $6.674 \times10^{-11}Nm^2 kg^{-2}$ ma per esigenze di simulazione viene qui aumentata di alcuni ordini di grandezza, $m_1$ e $m_2$ sono le masse dei corpi e $d^2$ è la distanza al quadrato tra i corpi.

In Unity viene assegnata questa forza ai corpi rigidi e sarà poi compito del modulo di fisica calcolare l'accelerazione gravitazionale per ogni corpo come segue:

### $$F_G=G \dfrac{m_1m_2}{d^2}=m_1a_g$$

### $$a_g=G \dfrac{m_2}{d^2}= \dfrac{F_G}{m_1}$$

La seconda equazione permette di calcolare la **velocità orbitale istantanea** di un corpo durante la sua **orbita kepleriana ellittica**:

### $$v^2= \mu \left ( \dfrac{2}{r}- \dfrac{1}{a} \right )$$

Quindi, per determinare la velocità istantanea abbiamo:

### $$v= \sqrt { \mu \left ( \dfrac{2}{r}- \dfrac{1}{a} \right )}$$

In questa equazione $v$ è la velocità istantanea del corpo nella sua orbita, $\mu$ è la **costante di gravitazione planetaria** e quando un corpo è molto più grande dell'altro (es. Sole-Terra) $\mu =GM$,  $r$ è la distanza del corpo da uno dei fuochi dell'ellisse occupato (in questo esempio dal Sole) e $a$ è il semiasse maggiore dell'ellisse.

La prima legge di Keplero afferma che _ogni pianeta si muove su orbite ellittiche ed il sole occupa uno dei fuochi dell'ellisse_. Il punto sulla traiettoria di un pianeta più lontano dal sole si chiama **Afelio** mentre il più vicino si chiama **Perielio**. La media tra Afelio e Perielio definisce il semiasse maggiore dell'ellisse. Le orbite ellittiche dei pianeti hanno una eccentricità molto bassa, quello della terra è $0.017$ e quindi il semiasse maggiore dell'ellisse è quasi uguale al raggio di un cerchio con il Sole al centro. In questo senso le orbite ellittiche dei pianeti possono essere approssimate a orbite circolari. Anche la Luna si muove su un'orbita ellittica rispetto alla Terra con il punto più lontano che si chiama **Apogeo** mentre quello più vicino **Perigeo**.

In un'orbita circolare $a=r$ la velocità tangenziale alla traiettoria è costante in modulo mentre l'accelerazione gravitazionale varia continuamente in direzione ma non influenza la velocità del corpo essendo normale alla velocità (moto circolare uniforme). **È proprio una ben precisa velocità tangenziale a mantenere il corpo in un'orbita circolare**. L'equazione della velocità orbitale istantanea può quindi essere semplificata per orbite circolari, in cui il semiasse maggiore è uguale alla distanza tra i corpi, come segue:

### $$a=r \to \left( \dfrac{2}{r} - \dfrac{1}{r} \right) = \left( \dfrac{1}{r} \right) \to v= \sqrt{ \mu \left( \dfrac{1}{r} \right )}= \sqrt{ \mu \left( \dfrac{1}{a} \right) }$$

In un'orbita ellittica $a \neq r$ **l'accelerazione gravitazionale varia continuamente in modulo e direzione modificando la velocità tangenziale in modulo e direzione** (accelerazione del corpo in prossimità del Perielio).

Quindi, per poter simulare in grafica 3D il moto orbitale planetario usando il motore di fisica di Unity è necessario rispettare le seguenti condizioni:

- I rapporti tra le masse dei corpi.
- I rapporti tra le distanze dei corpi.

In questa simulazione, quindi, ipotizzo quanto segue:

- La costante $G$ è aumentata di alcuni ordini di grandezza.
- La massa della Luna vale $1.0$.
- La massa di Mercurio è $4.5$ volte la massa lunare.
- La massa di Venere è $66.2$ volte la massa lunare.
- La massa della Terra è $81.3$ volte la massa lunare.
- La massa del Sole è $27069000.0$ volte la massa lunare.
- Le distanze in metri sono moltiplicate per un fattore $10^{-7}$
- Il Sole ha $v_i = 0$ e non risente della forza gravitazionale dei pianeti.
- La Terra e la Luna hanno velocità iniziale $v_i$ rispettivamente all'Afelio e all'Apogeo considerando l'Apogeo lunare in coincidenza dell'Afelio terrestre.
