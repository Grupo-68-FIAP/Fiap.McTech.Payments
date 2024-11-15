#coding:utf-8

Feature: Gerar QR-Code para pagamento
	Given um usuário
	deve ser capaz de gerar um código QR-Code
	para efetuar o pagamento do mesmo

	Scenario: Gerar um QR-Code para pagamento
		Given que eu tenho os dados válidos para gerar um QR-Code para pagamento
		When eu solicitar para a rota gerar o QR-Code
		Then o status de resposta deve ser 200 OK
		And o pagamento deve ser registrado no sistema