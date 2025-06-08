use kenshi_dnd_db;

INSERT INTO factions (name, description, baseRelations,color,respectByFighting) VALUES
('La Nación Sagrada', 'La Nación Sagrada es una teocracia fanática que venera a Okran,
 su deidad solar. Sus leyes son estrictas y su sociedad rechaza la tecnología antigua, 
 a la que consideran una herejía. Gobernada por inquisidores y paladines, impone su ideología 
 con mano de hierro y persigue sin piedad a quienes consideran impuros. Solo consideran puros a Humanos Greenlanders.',80,6,false),
('Ciudades Unidas', 'Las Ciudades Unidas son un conglomerado de estados esclavistas y comerciantes, d
ominados por una aristocracia corrupta. Su poder se basa en el comercio, la esclavitud y la riqueza. 
La vida humana tiene poco valor si no puede generar ganancia, y la ley solo sirve a los poderosos.',80,4,false),
('Shek Kingdom', 'El Reino Shek está formado por una orgullosa raza de guerreros con cuerpos endurecidos por el combate.
 Su cultura gira en torno al honor, la fuerza y la superación personal. Desprecian la cobardía y valoran la muerte en batalla 
 por encima de todo.',90,1,true),
 ('Canibales','Los canibales habitan en tribus por todo el norte de la isla. Practican la antrofagía como ritual sagrado, 
 y han perdido la capacidad del habla. No hacen excepciones, y te atacarán incluso si eres un esqueleto y no pueden comerte.',0,3,false),
 ('El Enjambre del Oeste','Sometidos a las feromónas de la Reina, los Enjambres actuan bajo una única voluntad. 
 El Enjambre del Oeste es una nación pacífica, y muy comercial. Mandan caravanas de enjambres a ambular el mundo en búsqueda de vender
 sus productos. Solo te atacarán si tienes un enjambre errante en tu facción.',80,5,false),
 ('Reino Animal','En la isla hay una gran variedad de animales. Expuestos al mundo post-apocalíptico de la isla, buscan comida
 donde pueden. Esa comida podrías ser tú.',0,2,false),
 ('Bandidos famélicos','Los bandidos de menos poder en toda la isla, buscarán a cualquiera al que linchar y robarle.',30,8,true),
 ('Esclavistas','Trabajan junto a las Ciudades Unidas y La Nación Sagrada para suplir su demanda de esclavos. Su modo de operación es simple, 
 trataran de derrotarte para después venderte.',10,7,false);
 
INSERT INTO regions(name, description, hasBar, hasShop, hasLimbHospital, hasContrabandMarket, hasRangedShop) VALUES
('Norte de la isla', 'No hay motivo por el que quieras estar aquí, más que para entrenar.', false, false, false, false, false),
('La Nación Sagrada', 'Serás atacado si no eres humano greenlander, y si lo eres, serás juzgado.', true, true, false, false, false),
('Ciudades Unidas', 'En un buen día, encontrarás mercaderes con grandes botínes, en uno malo, un noble querrá jugar a cazarte', true, true, false, false, true),
('Río del Enjambre del Oeste', 'Desean intercambiar bienes contigo, aunque la zona es conocida por tener una fauna peligrosa', false, true, true, false, false),
('Reino Shek', 'El reino más amistoso entre los reinos, solo querrán ponerse a prueba contigo por la gloria de Kral', true, true, false, false, false),
('Pantano', 'Encontrarás bienes exclusivos de contrabando en el lugar con más infecciones de toda la isla', true, true, true, true, false);

 insert into hostilities(hostility) values 
 ('OkranReligion'), -- Will attack everyone NOT Greenlander, and with robotical limbs
 ('CorruptOligarchy'), -- Might attack low income players, calculated by cats / hero
 ('StrengthTest'), -- Wants to fight powerful heroes, and will gain relations upon fighting them
 ('HiveCastOuts'), -- Will attack players with Hives
 ('Survival'); -- Will attack everything as a means to survive or gain money
 
 insert into faction_hostility(idFaction,idHostility) values
 (1,1),
 (2,2),
 (8,2),
 (3,3),
 (5,4),
 (4,5),
 (6,5),
 (7,5);
 
 insert into region_faction(regionId,factionId) values
 -- North of the island: Slavers, cannibals
 (1,4),
 (1,8),
 -- Holy nation: Holy nation, slavers
 (2,1),
 (2,8),
 -- United cities: United cities, slavers, animal kingdom
 (3,2),
 (3,6),
 (3,8),
 -- Western hive river: Western hive, animal kingdom
 (4,5),
 (4,6),
 -- Shek kingdom: Shek kingdom, starving bandits
 (5,3),
 (5,7),
 -- Pantano: Animal kingdom, starving bandits
 (6,6),
 (6,7);
 
 Insert into stats(bruteForce,dexterity,hp,resistance,agility) values
 -- Pure race
 (0,0,0,0,0), 
 -- Base Human
 (0,4,0,0,2), 
 -- Greenlander
(2,1,2,0,1), 
-- Scorchlander
(0,4, -2, 0,4),

-- Shek
(5,-2, 6, 7, -4), 
-- Skeleton
(3,3,5,3,-2),

-- Base Swarm
(1, 2, 1, 1, 1), 
-- Worker Swarm
(0, 3, 0, 0, 3), 
-- Prince Swarm
(0, 3, -1, 0, 4), 
-- Soldier Swarm
(3, -1, 3, 2, -1);

insert into races(name, stats_id) values
('Puro', 1),
('Humano', 2),
('Greenlander', 3),
('Scorchlander', 4),
('Shek', 5),
('Esqueleto', 6),
('Enjambre', 7),
('Dron Trabajador', 8),
('Príncipe', 9),
('Dron Soldado', 10);





insert into stats(bruteForce,dexterity,hp,resistance,agility) values
-- Ringed Saber: balanced, but loses some agility
(3, 2, 0, 1, 0), 
-- Fragment Axe: very heavy, severely penalizes agility
(7, -2, 0, 3, -3), 
-- Wakizashi: light weapon, the only one with +1 agility
(1, 5, 0, 0, 1), 
-- Hacker: blunt blade, some weight
(4, 1, 0, 2, -1), 
-- Paladin’s Cross: heavy weapon, strong penalty
(6, 0, 0, 4, -3), 
-- Horse Chopper:
(2, 3, 0, 1, 0), 
-- Desert Sabre: agile, but not as much as wakizashi
(3, 4, 0, 1, 0), 
-- Nodachi: great range, medium penalty
(5, 2, 0, 2, -2), 
-- Falling Sun: extremely heavy weapon
(8, -1, 0, 3, -4), 
-- Short Cleaver: robust and somewhat slow
(3, 2, 0, 2, -1), 
-- Plank: beastly in strength, but clumsy as a log
(9, -3, 0, 3, -5), 
-- Combat Cleaver: brutal blade, meant for slicing
(6, 1, 0, 3, -2), 
-- Topper: simple and functional, good blade
(3, 3, 0, 1, 0), 
-- Guardless Katana: faster but vulnerable
(2, 4, 0, 0, 1), 
-- Jitte: designed for disarming, not very agile
(2, 1, 0, 2, -1), 
-- Moon Cleaver: heavy but elegant
(5, 1, 0, 3, -2), 
-- Heavy Jitte: robust, not suitable for quick maneuvers
(6, 0, 0, 4, -3), 
-- Katana: light, but a bit heavier than the wakizashi
(2, 4, 0, 1, 0), 
-- Crescent Scythe: curved blade, somewhat clumsy
(4, 2, 0, 2, -1), 
-- Iron Stick: improvised and clumsy
(2, -1, 0, 1, -2);

INSERT INTO items (name, description, value, resellValue, type, weight, stats_id) VALUES
('Ringed Saber', 'Una espada curva balanceada, común entre mercenarios.', 500, 300, 'melee', 2, 11),
('Fragment Axe', 'Un hacha inmensa que puede partir en dos a un enemigo.', 1200, 800, 'melee', 4, 12),
('Wakizashi', 'Hoja corta y ligera, excelente como arma secundaria.', 250, 150, 'melee', 1, 13),
('Hacker', 'Hoja recta y pesada diseñada para atravesar armaduras.', 600, 400, 'melee', 3, 14),
('Paladin’s Cross', 'Espada bendita usada por paladines. Muy pesada.', 1800, 1200, 'melee', 4, 15),
('Horse Chopper', 'Corta fácilmente extremidades, ligera y común.', 400, 250, 'melee', 2, 16),
('Desert Sabre', 'Hoja curva adaptada al combate en desiertos.', 450, 300, 'melee', 2, 17),
('Nodachi', 'Espada muy larga, buena para cargar en campo abierto.', 800, 500, 'melee', 2, 18),
('Falling Sun', 'Un mandoble inmenso capaz de aniquilar enemigos.', 2000, 1500, 'melee', 4, 19),
('Short Cleaver', 'Cuchilla de batalla pequeña pero brutal.', 550, 350, 'melee', 2, 20),
('Plank', 'Arma brutal hecha con un trozo de hierro reforzado.', 1500, 1000, 'melee', 4, 21),
('Combat Cleaver', 'Gran cuchilla utilizada en escaramuzas de élite.', 900, 600, 'melee', 3, 22),
('Topper', 'Arma sencilla, bien balanceada, común entre reclutas.', 350, 200, 'melee', 2, 23),
('Guardless Katana', 'Versión sin guardamano, más rápida pero vulnerable.', 300, 180, 'melee', 2, 24),
('Jitte', 'Diseñada para bloquear y controlar, no matar.', 280, 160, 'melee', 2, 25),
('Moon Cleaver', 'Pesada hoja con forma de luna, elegante y letal.', 1100, 750, 'melee', 4, 26),
('Heavy Jitte', 'Versión reforzada del jitte, más pesada.', 500, 300, 'melee', 3, 27),
('Katana', 'La clásica espada curva, versátil y efectiva.', 600, 400, 'melee', 3, 28),
('Crescent Scythe', 'Hoja curvada con mucho estilo, pero algo torpe.', 850, 600, 'melee', 3, 29),
('Iron Stick', 'Una simple barra de hierro improvisada.', 100, 50, 'melee', 1, 30);


INSERT INTO meleeItems (item_id, breaksOnUse, canRevive) VALUES
(1, false, false),
(2, false, false),
(3, false, false),
(4, false, false),
(5, false, false),
(6, false, false),
(7, false, false),
(8, false, false),
(9, false, false),
(10, false, false),
(11, false, false),
(12, false, false),
(13, false, false),
(14, false, false),
(15, false, false),
(16, false, false),
(17, false, false),
(18, false, false),
(19, false, false),
(20, false, false);


-- Spring Bat: easy to use, light
-- Toothpick: light and fast
-- Eagle’s Cross: precise but heavy
-- Oldworld Bow MkII: powerful, heavy
-- Harpoon Gun: fixed, almost immobile
INSERT INTO stats (bruteForce, dexterity, hp, resistance, agility) VALUES
(1, 4, 0, 1, 0),  
(1, 5, 0, 1, 1),  
(2, 4, 0, 2, -1), 
(3, 3, 0, 3, -2), 
(4, 2, 0, 4, -3); 
INSERT INTO items (name, description, value, resellValue, type, weight, stats_id) VALUES
('Spring Bat', 'Ballesta rudimentaria de corto alcance.', 600, 400, 'ranged', 2, 31),
('Toothpick', 'Ballesta ligera con buena velocidad de disparo.', 750, 500, 'ranged', 1, 32),
('Eagle’s Cross', 'Ballesta de largo alcance, muy precisa.', 1200, 800, 'ranged', 3, 33),
('Oldworld Bow MkII', 'Una ballesta militar mejorada de gran daño.', 1500, 1000, 'ranged', 4, 34),
('Harpoon Gun', 'Arma pesada de defensa estática.', 2000, 1500, 'ranged', 4, 35);


INSERT INTO rangedItems (item_id, difficulty, maxAmmo) VALUES
(21, 1, 15),   
(22, 2, 20),   
(23, 4, 10),    
(24, 5, 8),    
(25, 3, 16);   




-- Standard First Aid Kit
-- Splint Kit
-- Professional First Aid
-- Bread Crumb
-- Phoenix Tear (high dexterity and physical effort)
-- Simple Meal: Strength boost
-- Mug of Water: Increases resistance
-- Meat Wrap: Boosts agility and energy
-- Raw Meat: Increases strength
-- Herbal Tea: Increases resistance
-- Stew: Increases strength and resistance
-- Hemp Beer: Increases resistance, reduces agility
-- Dustwich: Increases agility
-- Toxin Purge: Removes poison, increases resistance
-- Grog: Increases resistance, reduces agility
-- Hashish: A prohibited drug on almost the entire island

INSERT INTO stats (bruteForce, dexterity, hp, resistance, agility) VALUES
( 0, 0, 5, 0, 0),  
( 0, 0, 5, 4, -2), 
( 0, 0, 10, 2, 0), 
( 1, 1, 2, 1, 1),  
( 2, 2, 0, 2, -1), 
( 2, 0, 0, 0, 0),  
( 0, 0, 0, 2, 0),  
( 0, 1, 0, 0, 1),  
( 2, 0, 0, 0, 0),  
( 0, 0, 0, 2, 0),  
( 3, 0, 5, 3, 0),  
( 0, 0, 0, 2, -1), 
( 0, 1, 0, 0, 2),  
( 0, 0, 0, 3, -2),  
( 0, 0, 0, 4, -2), 
(-2,-2,-2,-2,6);


INSERT INTO items (name, description, value, resellValue, type, weight, stats_id) VALUES
('Primeros auxilios estándar', 'Kit básico de primeros auxilios. Se consume al usarse.', 200, 100, 'melee', 1, 36),
('Ferula basica', 'Inmoviliza miembros fracturados. Se consume al usarse.', 150, 75, 'melee', 1, 37),
('Primeros auxilios profesional', 'Restaura sangre rápidamente. Se consume al usarse.', 300, 150, 'melee', 1, 38),
('Mendrugo de pan', 'La única comida que sobrevivió el cataclismo.Se consume al usarse.', 200, 50, 'melee', 1, 39),
('Ron de sangre', 'Su nombre es un misterio, y su elaboración, también. Es capaz de revivir a un aliado. Se consume al usarse.', 2000, 1000, 'melee', 1, 40),
('Comida Simple', 'Comida básica que satisface el hambre y otorga un ligero aumento en la fuerza.', 50, 25, 'melee', 1, 41),
('Taza de agua', 'Agua potable que hidrata y mejora la resistencia física temporalmente.', 10, 5, 'melee', 1, 42),
('Wrap de carne', 'Parece carne cruda envuelta en tela, aumenta la energía y la agilidad.', 80, 40, 'melee', 1, 43),
('Comida cruda', 'Carne cruda, otorga un pequeño aumento de fuerza.', 100, 50, 'melee', 1, 44),
('Té herbal', 'Bebida herbal que mejora la calma y recarga la resistencia.', 150, 75, 'melee', 1, 45),
('Guiso', 'Un guiso que cubre bien el hambre y mejora la resistencia y fuerza.', 200, 100, 'melee', 2, 46),
('Cerveza de cáñamo', 'Cerveza artesanal que alivia el cansancio, pero reduce la agilidad.', 120, 60, 'melee', 1, 47),
('Dustwich', 'Un sándwich con polvo del desierto, ligeramente mejora la agilidad.', 60, 30, 'melee', 1, 48),
('Sake', 'Poca cosa pero matón, otorga un aumento de resistencia temporal a costa de equilibrio.', 300, 150, 'melee', 1, 49),
('Grog', 'Bebida alcohólica que aumenta la resistencia, pero reduce la agilidad.', 250, 125, 'melee', 1, 50),
('Hashish', 'Tremendamente ilegal, te dejará en con el corazón a mil.', 700, 600, 'melee', 1, 51);


INSERT INTO meleeItems (item_id, breaksOnUse, canRevive) VALUES
(26, true, false),  
(27, true, false),  
(28, true, false),  
(29, true, false),  
(30, true, true), 
(31, true, false),  
(32, true, false),  
(33, true, false),  
(34, true, false),  
(35, true, false),  
(36, true, false),  
(37, true, false),  
(38, true, false),  
(39, true, false),  
(40, true, false),
(41, true, false);  

INSERT INTO stats (bruteForce, dexterity, hp, resistance, agility) VALUES
(0, 2, 2, -2, 4),  
(4, -2, 4, 4, -2), 
(4, 4, 0, 1, 1),   
(-2, -2, 2, 8, -2),
(1, 6, 0, -1, -2); 
INSERT INTO limbs (name,value, stats_id) VALUES
('Pierna ágil',1000, 52),
('Extremidad de metal',1200, 53),
('Extremidad de Kung-Fu',1500, 54),
('Extremidad Antigolpes',1000, 55),
('Brazo de francotirador',1500, 56);


INSERT INTO enemies (factionId, name, health, strength, resistance, agility, immunity, xp, maxCatDrop, canDropItem) VALUES
-- Animal kingdom (6)
(6, 'Cabra', 3, 4, 1, 4, 'None', 25, 0, false),
(6, 'Garral Joven', 6, 5, 3, 8, 'ResistantToMelee', 75, 0, false),
(6, 'Bicho de las Dunas', 5, 4, 3, 6, 'ResistantToBoth', 100, 0, false),
(6, 'Perro huesudo', 4, 3, 2, 6, 'None', 60, 0, false),
(6, 'Garral', 6, 6, 4, 7, 'ImmuneToMelee', 150, 0, false),

-- Cannibals (4)
(4, 'Caníbal Desnutrido', 3, 2, 2, 5, 'None', 50, 50, true),
(4, 'Caníbal Cazador', 4, 3, 2, 6, 'None', 75, 75, true),
(4, 'Chamán Caníbal', 5, 2, 3, 4, 'ResistantToRanged', 120, 100, true),
(4, 'Jefe Caníbal', 6, 5, 4, 5, 'ResistantToBoth', 250, 200, true),
(4, 'Caníbal Loco', 4, 4, 2, 6, 'None', 90, 80, true),

-- Starving bandits (7)
(7, 'Bandido Famélico', 2, 2, 2, 5, 'None', 40, 30, true),
(7, 'Bandido Famélico Fuerte', 3, 3, 2, 5, 'None', 60, 50, true),
(7, 'Líder Bandido Famélico', 3, 3, 3, 5, 'None', 80, 75, true),

-- Holy nation (1)
(1, 'Recluta de Okran', 4, 3, 3, 4, 'None', 300, 300, true),
(1, 'Fanático de Okran', 4, 4, 3, 5, 'None', 400, 400, true),
(1, 'Paladín Sagrado', 6, 6, 4, 4, 'ResistantToBoth', 1000, 1000, true),
(1, 'Inquisidor', 8, 7, 5, 3, 'ImmuneToRanged', 2000, 2000, true),

-- United cities (2)
(2, 'Soldado de las Ciudades Unidas', 5, 5, 4, 5, 'ResistantToMelee', 800, 800, true),
(2, 'Guardia de Seguridad', 6, 6, 4, 4, 'ImmuneToMeleeAndResistantToRanged', 1000, 1200, true),
(2, 'Oficial de Patrulla', 6, 6, 4, 4, 'ResistantToBoth', 950, 1000, true),

-- Shek kingdom (3)
(3, 'Guerrero Shek', 5, 6, 5, 5, 'ResistantToBoth', 800, 1000, true),
(3, 'Veterano Shek', 6, 6, 5, 5, 'ResistantToBoth', 1000, 1200, true),
(3, 'Campeón Shek', 6, 7, 6, 4, 'ImmuneToMelee', 1500, 1500, true),

-- Western hive (5)
(5, 'Errante del Enjambre', 3, 3, 3, 6, 'None', 200, 100, true),
(5, 'Guerrero del Enjambre', 4, 5, 4, 7, 'ResistantToRanged', 400, 300, true),
(5, 'Príncipe del Enjambre', 5, 4, 4, 6, 'None', 500, 400, true),
(5, 'Artillero del Enjambre', 3, 3, 3, 5, 'None', 350, 250, true),

-- Slavers (8)
(8, 'Cazador Esclavista', 4, 4, 3, 6, 'None', 600, 500, true),
(8, 'Mercader Esclavista', 3, 3, 2, 4, 'None', 300, 400, true),
(8, 'Capataz Esclavista', 5, 5, 3, 5, 'ImmuneToRanged', 1000, 800, true),
(8, 'Esclavista Veterano', 6, 6, 4, 4, 'ResistantToBoth', 1200, 1000, true);


INSERT INTO names (name) VALUES
('Esata'),
('Squin'),
('Cat-Lon'),
('Tinfist'),
('Holy Phoenix'),
('Okran'),
('Simone'),
('Bugmaster'),
('Agnu'),
('Beep'),
('Seto'),
('Miu'),
('Ruka'),
('Greenfinger'),
('Rain the Giant'),
('Kang'),
('Burn'),
('Sadneil'),
('Oron'),
('Moll'),
('Longen'),
('Soto'),
('Nux'),
('Crimper'),
('Stobe'),
('Dimak'),
('Barka'),
('Green'),
('Sarn'),
('Griffin'),
('Chad'),
('Owl'),
('Grey'),
('Bo'),
('Mu'),
('Bishop'),
('Bull'),
('Void'),
('Caliburn'),
('Ghost'),
('Fang'),
('Buzzo'),
('Rane'),
('Okra'),
('Ray'),
('Juno'),
('Patch'),
('Slug'),
('Zan'),
('Twinkle');

INSERT INTO titles (title) VALUES
('El Lobo'),
('La Espina'),
('El Rápido'),
('La Hoja'),
('El Fuerte'),
('La Tormenta'),
('El Silencio'),
('La Sombra'),
('El Sabio'),
('El Fénix'),
('El Viejo Lobo'),
('La Mano Justa'),
('El Errante'),
('La Furia'),
('El Ojo Frío'),
('La Voz Gris'),
('El Sable'),
('El Hierro'),
('La Piedra'),
('El Último'),
('El Flojo'),
('La Rata'),
('El Tullido'),
('El Tuerto'),
('La Carga'),
('El Estorbo'),
('El Rencor'),
('La Sanguijuela'),
('El Perdido'),
('La Sombra Rota'),
('El Mudo'),
('La Peste'),
('El Cobarde'),
('La Ruina'),
('El Sin Nombre'),
('La Mancha'),
('El Fracasado'),
('El Chiflado'),
('La Vergüenza'),
('El Muerto en Vida');


INSERT INTO backgrounds (background) VALUES
('Ex-esclavo del Gremio de la Cadena. Lleva las marcas aún en la espalda. Silencioso, pero letal con la lanza.'),
('Expulsado de los monjes de polvo por beber sangre en lugar de agua. Dice que fue por fe, pero nadie le cree.'),
('Antigua mercenaria de las Llamas Rojas. Perdió su escuadrón y desde entonces bebe más de lo que duerme.'),
('Cazador de bandidos venido a menos. Perdió una pierna, pero la remplazó con metal y rabia.'),
('Crió ratas del desierto durante diez años. Las comía. También las entrenaba. Dice que aprendió paciencia.'),
('Ex-guardia de ciudad caída. Se llevó la armadura, y la culpa. Quiere redención, o algo que se le parezca.'),
('Fue carnicero, hasta que las carnes empezaron a moverse. Desde entonces solo corta cosas que sangren.'),
('Nacido en las ruinas. Nunca ha visto una ciudad. Pelea como si cada día fuera el último, porque para él lo es.'),
('Hermano menor de un señor de la guerra. Se negó a matar por poder y huyó. Ahora mata por comida.'),
('Superviviente de un convoy perdido en las arenas. Fue el único que regresó, y no habla de cómo.'),
('Hija de una comerciante y un asesino. Sabe negociar y apuñalar por igual. Cobra por ambos servicios.'),
('Criado por bandidos, luego traicionado por ellos. Ahora caza a los suyos por deporte y oro.'),
('Poeta fracasado, luchador inesperado. Escribe versos en la sangre que derrama.'),
('Fue esclavo gladiador durante seis años. Cuando lo soltaron, mató al dueño. No ha parado desde entonces.'),
('Nadie sabe su nombre real. Dice que fue sombra de un rey, y ahora busca su propia historia.'),
('Comió carne humana durante una hambruna. No le enorgullece, pero tampoco se disculpa.'),
('Hombre tranquilo, manos violentas. Nunca grita, nunca corre. Mata despacio, pero seguro.'),
('Expulsada de tres bandas por "problemas de actitud". Ella dice que era la única cuerda.'),
('Cree que una voz le habla desde las montañas. Le obedece. A veces esa voz tiene razón.'),
('Peleó con un palo hasta que le dieron una espada. Siguió usando el palo. "Más confiable", dice.'),
('Fue minero en las Tierras Negras. Un derrumbe lo dejó atrapado una semana. Desde entonces, odia los túneles y ama el cielo.'),
('Ladrón reformado, o eso dice. Todavía camina como si tuviera que escapar en cualquier momento.'),
('Creció en una caravana de mercaderes nómadas. Perdió a su familia por fuego cruzado. Desde entonces, solo viaja solo.'),
('Dice haber sido criado por un anciano espadachín ciego. Pelea como si el mundo fuera niebla.'),
('Sirvió como guardia en un burdel de frontera. Aprendió más sobre gente que sobre lucha. Pero también aprendió a pelear.'),
('Era aprendiz de médico. Un error mató a un niño. No volvió a tocar vendas, solo espadas.'),
('Vivió entre caníbales durante tres años. Nunca dice si fue prisionero o parte de la tribu.'),
('Camina con muletas, pero pelea como si no las necesitara. Dice que cada golpe le recuerda por qué sigue vivo.'),
('Limpió cadáveres en el campo de batalla desde los doce años. Un día, agarró un arma y no soltó más ninguna.'),
('Habla solo. A veces ríe, a veces llora. A veces su "otro yo" pelea mejor que él.'),
('Se unió a un culto por amor. Salió cuando el amor fue sacrificado. Guarda una cicatriz en el pecho, y otra en el alma.'),
('Era carcelero. Dejó libre a un prisionero inocente. Perdió el trabajo, pero ganó conciencia. Ahora busca justicia a su manera.'),
('Cazador de bestias que perdió la vista de un ojo. “Ahora solo veo lo importante”, dice mientras apunta.'),
('Viajó con músicos ambulantes. Tocaba el tambor. Ahora solo toca tambores de guerra.'),
('Fue criado para ser noble, pero se negó a mandar. Prefirió vivir entre polvo y barro que dar órdenes injustas.'),
('Tuvo un hijo. Lo perdió en una redada. Desde entonces, no se le ha visto sonreír.'),
('Era cocinero en una fortaleza. Envenenó a los invasores. Desde entonces cocina para soldados… y asesinos.'),
('Aprendió a luchar viendo peleas de gallos. Dice que todo lo que sabe lo aprendió de un ave furiosa.'),
('Trabajó como enterrador. Enterró enemigos y amigos por igual. Dice que cada muerte lo hizo más fuerte.'),
('Tenía una promesa, una causa, un líder. Ya no le queda nada de eso. Solo una espada y ganas de seguir.'),
('Estuvo encerrado en Rebirth durante cinco años. Aprendió a sobrevivir comiendo piedras… o algo parecido.'),
('Caminó solo desde Mongrel hasta Black Scratch. No por valentía, sino porque no quedaba nadie más.'),
('Defendió un puesto comercial en The Hook durante el asedio de los Skin Bandits. Sobrevivió a costa de los otros.'),
('Fue maestro de espadas en World''s End. Dejó todo atrás tras un duelo perdido que aún sueña cada noche.'),
('Limpió sangre en las arenas de gladiadores de Sho-Battai. Ahora cobra por derramarla, no por limpiarla.'),
('Vivió en las minas de Clownsteady sin ver el cielo durante años. Ahora no entra en cuevas, ni aunque haya oro.'),
('Sirvió como escolta de caravanas entre Heft y Stoat. La mitad de su rostro quedó en una emboscada. La otra mitad sonríe.'),
('Perdió un brazo en los campos de Vain. No quiere hablar del cómo. Solo quiere una nueva oportunidad.'),
('Fue traicionado por sus propios aliados cerca de Blister Hill. Desde entonces, duerme con un ojo abierto y una daga bajo el cuello.'),
('Pasó una temporada en las ruinas de la Nube Roja. Dice que no encontró enemigos… solo recuerdos.'),
('Estuvo al servicio de un noble de Brink. Vio el lujo pudrirse desde adentro. Ahora prefiere el polvo bajo las botas.'),
('Habitó en las sombras de Stack, fingiendo fe para no arder. Hoy solo cree en su espada.'),
('Durante años, rastreó esclavistas cerca de Port North. No por justicia, sino por venganza.'),
('Vigiló las puertas de Heng en los días de disturbio. Aprendió que los muros no detienen la traición.'),
('Abandonó una torre de investigación en Black Desert City. Dice que la ciencia no basta cuando el mundo se cae a pedazos.'),
('Cruzó el Mar de Acido con un grupo. Solo volvió uno. Nunca dijo qué ocurrió, pero ya no teme al dolor.'),
('Construyó una cabaña cerca de Leviathan Coast. Vivió en paz hasta que algo lo obligó a volver a empuñar el filo.'),
('Presenció una ejecución en Okran''s Shield. No fue la víctima… pero ese día murió algo en él.'),
('Exploró las ruinas bajo Bast. Lo que encontró no puede decirlo, pero desde entonces duerme mal.'),
('Sirvió a un líder de Deadcat. Lo traicionaron. Ahora trabaja solo, y no pregunta mucho.');
