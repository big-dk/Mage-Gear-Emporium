; always use mqg
(assert (= (id slot-12-2) 19339))

;; can't use same ring or same trinket in two slots.
(assert (not (= (id slot-11-1) (id slot-11-2))))
(assert (not (= (id slot-12-1) (id slot-12-2))))

(declare-const weaponhit Int)
(assert (= weaponhit 
	(if 
		(= usestaffs 1)
		(spellHit slot-17)
		(+ 
			(spellHit slot-13)
			(spellHit slot-23)
		)
	)
))
(declare-const weapondamage Int)
(assert (= weapondamage 
	(if 
		(= usestaffs 1)
		(spellDamage slot-17)
		(+ 
			(spellDamage slot-13)
			(spellDamage slot-23)
		)
	)
))


(assert (>= 8 (+
	(spellHit slot-1)
	(spellHit slot-2)
	(spellHit slot-3)
	;; 4 skip
	(spellHit slot-5)
	(spellHit slot-6)
	(spellHit slot-7)
	(spellHit slot-8)
	(spellHit slot-9)
	(spellHit slot-10)
	(spellHit slot-11-1)
	(spellHit slot-11-2)
	(spellHit slot-12-1)
	(spellHit slot-12-2)
	;; 14 skip
	(spellHit slot-15)
	(spellHit slot-16)
	; (spellHit slot-26)
	
	weaponhit

	;; staff
	;(spellHit slot-17)

	;; mainhand offhand
	;(spellHit slot-13)
	;(spellHit slot-23)

	; zanzil
	(if 
		(and 
			(= (id slot-11-1) 19905)
			(= (id slot-11-2) 19893)
			)
		1
		0
	)
)))

(declare-const total-spev Int)
(assert (= total-spev (+
	(spellDamage slot-1)
	(spellDamage slot-2)
	(spellDamage slot-3)
	; 4 skip
	(spellDamage slot-5)
	(spellDamage slot-6)
	(spellDamage slot-7)
	(spellDamage slot-8)
	(spellDamage slot-9)
	(spellDamage slot-10)
	(spellDamage slot-11-1)
	(spellDamage slot-11-2)
	(spellDamage slot-12-1)
	(spellDamage slot-12-2)
	;; 14 skip
	(spellDamage slot-15)
	(spellDamage slot-16)

	; (spellDamage slot-26)
	
	weapondamage

	;; mh/offhand
	;(spellDamage slot-13)
	;(spellDamage slot-23)

	; staff
	;(spellDamage slot-17)

	;; bloodvine
	(if 
		(and (= (id slot-5) 19682) (= (id slot-7) 19683)  (= (id slot-8) 19684) )
		(* 2 spellcrit-value)
		0
	)

	;; pvp 2set
	(if 
		(and (= (id slot-3) 23319) (= (id slot-8) 23291))
		23000
		0
	)

	; zanzil
	(if 
		(and 
			(= (id slot-11-1) 19905)
			(= (id slot-11-2) 19893)
			)
		6000
		0
	)
)))
