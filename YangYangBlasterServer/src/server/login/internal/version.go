package internal

import (
	"encoding/json"
	"fmt"
	"os/exec"
)

type Version struct {
	Android_Live string `json:"Android_Live"`
	Android_Test string `json:"Android_Test"`
	IOS_Live     string `json:"IOS_Live"`
	IOS_Test     string `json:"IOS_Test"`
}

func GetVersionFile() Version {

	cmd := exec.Command("curl", "https://raw.githubusercontent.com/agewsj/YangYangBlaster/master/Version.json")

	fmt.Println(cmd)

	output, err := cmd.Output()
	if err != nil {
		panic(err)
	}

	// s := `{
	// 	"Android_Live": "1.0.1_1",
	// 	"Android_Test": "1.0.1_2",
	// 	"IOS_Live": "1.0.1_3",
	// 	"IOS_Test": "1.0.1_4"
	//   }`
	version := Version{}
	json.Unmarshal([]byte(output), &version)

	fmt.Println(string(output))
	fmt.Println(version)

	return version
}
