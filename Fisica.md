# Moto orbitale in Unity

![](https://dl.dropboxusercontent.com/s/ziekoxun8flpd8o/diagram3.png?dl=1)

### Fisica
Questa semplice simulazione del moto orbitale dei pianeti viene eseguita usando solo il motore del modulo _built-in_ di fisica di Unity. Non viene eseguito nessun calcolo della traiettoria orbitale ma la traiettoria è il risultato dell'applicazione di forze e velocità ai corpi rigidi considerati. Si considera solo il moto di rivoluzione e non quello di rotazione.

In questa simulazione vengono usate solo due equazioni della meccanica classica.

![](https://dl.dropboxusercontent.com/s/ziekoxun8flpd8o/diagram3.png?dl=1)

La prima è la **legge di gravitazione universale di Newton**

$$F_G=G\dfrac{m_1m_2}{d^2}=m_1a$$

Questa legge afferma che la forza gravitazionale $$F_G$$ generata tra due corpi nello spazio è direttamente proporzionale al prodotto delle masse dei corpi e inversamente proporzionale al quadrato della distanza tra i centri di massa dei corpi. $$G$$ è la costante di gravitazione universale e vale $$6.674\times10^{-11}Nm^2 kg^{-2}$$ ma per esigenze di simulazione viene qui aumentata, $$m_1$$ e $$m_2$$ sono le masse dei corpi e $$d^2$$ è la distanza al quadrato tra i corpi.

In Unity viene assegnata questa forza ai corpi rigidi e sarà poi compito del modulo di fisica calcolare l'accelerazione gravitazionale per ogni corpo come segue:

$$a=G\dfrac{m_2}{d^2}=\dfrac{F_G}{m_1}$$

La seconda equazione permette di calcolare la **velocità orbitale istantanea** di un corpo durante la sua **orbita kepleriana ellittica**. In questa simulazione si usano orbite ellittiche per le traiettorie dei pianeti intorno al Sole.


![](https://dl.dropboxusercontent.com/s/ziekoxun8flpd8o/diagram3.png?dl=1)
