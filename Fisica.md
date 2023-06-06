# Moto orbitale in Unity

![](https://dl.dropboxusercontent.com/s/ziekoxun8flpd8o/diagram3.png?dl=1)

### Fisica
Questa semplice simulazione del moto orbitale dei pianeti viene eseguita usando solo il motore del modulo _built-in_ di fisica di Unity. Non viene eseguito nessun calcolo della traiettoria orbitale ma la traiettoria è il risultato dell'applicazione di forze e velocità ai corpi rigidi considerati. Si considera solo il moto di rivoluzione e non quello di rotazione.

In questa simulazione vengono usate solo due equazioni della meccanica classica.

![](https://dl.dropboxusercontent.com/s/ziekoxun8flpd8o/diagram3.png?dl=1)

La prima è la **legge di gravitazione universale di Newton**:
$$F_G=G\dfrac{m_1m_2}{d^2}=m_1a$$
Questa legge afferma che la forza gravitazionale $$F_G$$ generata tra due corpi nello spazio è direttamente proporzionale al prodotto delle masse dei corpi e inversamente proporzionale al quadrato della distanza tra i centri di massa dei corpi. $$G$$ è la costante di gravitazione universale e vale $$6.674\times10^{-11}Nm^2 kg^{-2}$$ ma per esigenze di simulazione viene qui aumentata, $$m_1$$ e $$m_2$$ sono le masse dei corpi e $$d^2$$ è la distanza al quadrato tra i corpi.

In Unity viene assegnata questa forza ai corpi rigidi e sarà poi compito del modulo di fisica calcolare l'accelerazione gravitazionale per ogni corpo come segue:
$$a=G\dfrac{m_2}{d^2}=\dfrac{F_G}{m_1}$$

La seconda equazione permette di calcolare la **velocità orbitale istantanea** di un corpo durante la sua **orbita kepleriana ellittica**. In questa simulazione si usano orbite ellittiche per le traiettorie dei pianeti intorno al Sole.
$$v^2=\mu\left(\dfrac{2}{r}-\dfrac{1}{a}\right)$$

In questa equazione $$\mu$$ è la **costante di gravitazione planetaria** la quale, quando un corpo è molto più grande dell'altro, vale:
$$\mu=GM$$

Quindi, per determinare la velocità istantanea abbiamo:
$$v=\sqrt{\mu\left(\dfrac{2}{r}-\dfrac{1}{a}\right)}$$
In questa equazione $$v$$ è la velocità istantanea del corpo nella sua orbita, $$\mu$$ la costante di gravitazione planetaria che può semplificarsi in $$GM$$ nel caso un corpo abbia una massa molto più grande dell'altro (es. Sole-Terra), $$r$$ è la distanza del corpo da uno dei fuochi occupati (in questo esempio dal Sole) e $$a$$ è il semiasse maggiore dell'ellisse.

La prima legge di Keplero afferma che _ogni pianeta si muove su orbite ellittiche ed il sole occupa uno dei fuochi dell'ellisse_. Il punto sulla traiettoria della Terra più lontano dal sole si chiama **Afelio** mentre il più vicino si chiama **Perielio**. la media tra Afelio e Perielio definisce il semiasse maggiore dell'ellisse. Le orbite ellittiche dei pianeti hanno una eccentricità molto bassa, quello della terra è $$0.017$$ e quindi il semiasse maggiore dell'ellisse è quasi uguale al raggio di un cerchio con il Sole al centro. In questo senso le orbite ellittiche dei pianeti possono essere approssimate a orbite circolari. Anche la Luna si muove su un'orbita ellittica rispetto alla Terra con il punto più lontano che si chiama **Apogeo** mentre quello più vicino **Perigeo**.

In un'orbita circolare la velocità tangenziale alla traiettoria è costante in modulo mentre l'accelerazione gravitazionale varia continuamente in direzione ma non influenza la velocità del corpo essendo normale alla velocità. **È proprio una ben precisa velocità tangenziale a mantenere il corpo in un'orbita circolare**. L'equazione della velocità orbitale istantanea può quindi essere semplificata per orbite circolari, in cui il semiasse maggiore è uguale alla distanza tra i corpi, come segue:
$$a=r\to\dfrac{2}{r}-\dfrac{1}{r}=\dfrac{1}{r}\to v=\sqrt{\dfrac{\mu}{r}}=\sqrt{\dfrac{\mu}{a}}$$

In un'orbita ellittica l'accelerazione gravitazionale varia continuamente in modulo e direzione modificando la velocità tangenziale in modulo e direzione.

Quindi per poter simulare in grafica 3D il moto orbitale planetario usando il motore di fisica di Unity è necessario rispettare le seguenti condizioni:


![](https://dl.dropboxusercontent.com/s/ziekoxun8flpd8o/diagram3.png?dl=1)
