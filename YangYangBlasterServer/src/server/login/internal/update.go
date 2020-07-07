package internal

import (
	"time"
)

var ticker = time.NewTicker(5 * time.Second)
var quit = make(chan struct{})

func InitUpdate() {
	GetVersionFile()
	skeleton.Go(func() {
		for {
			select {
			case <-ticker.C:
				// do stuff
				//fmt.Println("ticker!!")
			case <-quit:
				ticker.Stop()
				return
			}
		}
	}, func() {

	})
}

func DestroyUpdate() {
	close(quit)
}
