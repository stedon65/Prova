# Moto orbitale in Unity

### Codice
Per eseguire diversi tipi di simulazione ho creato una classe **SpatialEntity** che permetta di rappresentare, a livello di dominio, una generica entità dello spazio dotata di alcune proprietà fondamentali e che sia composta con una classe **GameObject** di Unity che rappresenta l'oggetto grafico 3D a sua volta composto con il componente necessario alla simulazione fisica ovvero la classe **Rigidbody**.

![](https://dl.dropboxusercontent.com/s/t8hfl2zwcl3ucix/Code1?dl=1)

Nel costruttore della classe viene chiamato il metodo **CreateObject** per creare l'istanza del **GameObject** partendo dalla primitiva **Sphere** a cui viene aggiunto il componente **Rigidbody**.

La classe **Rigidbody** consente a Unity di controllare direttamente il moto dell'oggetto grafico tramite il motore di fisica built-in. Per queste simulazioni, tuttavia, la proprietà **useGravity** viene disabilitata in quanto la forza gravitazionale viene calcolata direttamente dalla classe **SpatialEntity**.

Successivamente viene chiamato il metodo **AddTrail** che aggiunge il componente **TrailRenderer** per disegnare la traccia durante il moto orbitale dei pianeti.

![](https://dl.dropboxusercontent.com/s/devsn0p2wsgabci/Code2.png?dl=1)

![](https://dl.dropboxusercontent.com/s/a8xxvxwems28zyn/Code3.png?dl=1)

![](https://dl.dropboxusercontent.com/s/ic4uv62natogn8s/Code4.png?dl=1)




