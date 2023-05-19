# Simulazione di moto orbitale in Unity


### Architettura
Uno dei principi più importanti di design Object-Oriented afferma che è meglio _favorire la composizione rispetto all'ereditarietà_. Questa affermazione ha come focus primario la manutenibilità del software e implicitamente si riferisce ad un altro importante principio che esorta a _programmare verso le interfacce e non verso l'implementazione_. Se l'obiettivo come sviluppatori è quello di raggiungere un alto disaccoppiamento e una alta riusabilità del codice l'applicazione di questi due principi ci conduce sulla retta via. 

Unity architetturalmente abbraccia questa filosofia alla radice implementando una **architettura a componenti "plugin"**. L'obiettivo dello sviluppatore in Unity è quello di scomporre il dominio del problema in componenti il più possibile autonomi per poi raggruppare questi componenti in "contenitori componibili" rappresentanti le astrazioni importanti del dominio di simulazione. È importante soprattutto individuare _classi comportamentali_ da implementare come componenti riutilizzabili. In questo senso, pensare ad un design pattern come **Strategy** può aiutare molto. 

In Unity i componenti devono derivare da una classe base specifica, **MonoBehaviour** e questo permette al motore interno di poter effettuare alcune callback ridefinite in ogni componenete in alcuni momenti ben precisi. Tra i componenti nativi che interessano maggiormenete questa simulazione due sono i più importanti: **Rigidbody** e **Collider** e tra le callback **FixedUpdate()**.

### Design
Possiamo pensare lo spazio come ad una struttura ad albero formata da componenti. Un generico _componente_ dello spazio che rappresenta un _aggregato di materia_ è definito da una classe astratta chiamata **SpaceAggregate**. 

Dalla classe astratta possono quindi derivare alcune classi concrete, le foglie della struttura ad albero, come **Star** e **Planet** per definire stelle e pianeti come semplici aggregati di materia.

Un componente **SpaceAggregate** però può essere composto da ulteriori componenti **SpaceAggregate** per formare strutture composite speciali come **Galaxy** e **SolarSystem**.

L'aspetto importante si esplicita nel fatto che i client della struttura lavorano solo attraverso il comportamento definito a livello di componente astratto di **SpaceAggregate**. 

Per implementare questa semplice simulazione inizierò con l'applicare il **Design Pattern Composite**.

### Fisica
- tipi di fisica in unity
- i ruoli di rigidbody e collider
- fixedupdate vs. update

[![Watch the video](https://img.youtube.com/vi/<VIDEO_ID>/hqdefault.jpg)](https://www.dropbox.com/s/tbsu4gn1d56g75q/20230519131325.mp4?dl=0)
