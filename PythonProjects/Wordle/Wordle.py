import random
from typing import Tuple
from xml.dom.minidom import Notation
import pygame
from pygame.locals import *
from Alert import Alert
from Text import Text
from TextBox import TextBox
from colors import *
from Window import Window
from Surface import Surface
from Button import Button
from Style import Style


class Wordle:
	"""A class to play the game Wordle
	"""

	size = (400, 700)
	"""The size of the game window"""
	width = size[0]
	"""The width of the game window"""
	height = size[1]
	"""The height of the game window"""


	caption = "WORDLE"
	"""The caption for the game window"""
	backgroundColor = (23, 23, 23)
	"""The background color for the game window"""
	clock = pygame.time.Clock()
	"""The clock for the game to keep track of the time between each frame"""
	font = "HelveticaNeueBold.ttf"
	"""The font for the game"""

	acceptedWords = set(map(str.strip, open('FiveLetterWords.txt')))
	"""The accepted words for the game in a set"""

	acceptedWordsList = list(map(str.strip, open('FiveLetterWords.txt')))
	"""THe accepted words for the game in a list"""



	def __init__(self):
		"""Initializes the Game
		"""
		# Initializes pygame and creates the window
		pygame.init()
		self.window = Window(self.size, self.caption, self.backgroundColor)

	def Start(self):
		"""Shows the start screen for the game
		"""
		# create the startScreen		
		startScreen = Surface((0, 0), self.size, self.backgroundColor)

		# Create the title for the startScreen
		title = Text((self.width / 2, 100), self.font, 75, "WORDLE", WHITE)

		# Create the Log in button for the startScreen
		logInButton = Button(Surface((self.width/ 4 - 75 + 5, 550), (150, 50), self.backgroundColor),
							Style(Text((75, 25), self.font, 25, "LOG IN", DENIM),
							fillColor=self.backgroundColor, borderColor=DENIM, borderRadius=5),

                    		hoverStyle=Style(Text((75, 25), self.font, 26, "LOG IN", WHITE),
							fillColor=DENIM, borderColor=DENIM, borderRadius=5))

		# Create the sign up button for the startScreen
		signUpButton = Button(Surface((self.width * 3 / 4 - 75 - 5, 550), (150, 50), self.backgroundColor),
								Style(Text((75, 25), self.font, 24, "SIGN UP", ORCHID),
								fillColor=self.backgroundColor, borderColor=ORCHID, borderRadius=5),

								hoverStyle=Style(Text((75, 25), self.font, 25, "SIGN UP", WHITE),
								fillColor=ORCHID, borderColor=ORCHID, borderRadius=5))

		while True:
			# run at 60 fps
			self.clock.tick(60)

			# clear the screen
			startScreen.clear()

			# get the position of the mouse for later use
			mousePos = pygame.mouse.get_pos()

			# loop through the events
			for event in pygame.event.get():

				# Check for QUIT event
				if event.type == pygame.QUIT:
					return

				# Checks for the MOUSEDOWN event
				if event.type == pygame.MOUSEBUTTONDOWN:
					# if the mouse clicked the log in button
					if (logInButton.mouseIsHovering(mousePos)):
						self.LogIn()
						return

					# if the mouse clicked the sign up button
					elif (signUpButton.mouseIsHovering(mousePos)):
						self.SignUp()
						return

			# render the background rectangles
			pygame.draw.rect(startScreen.display, TURQUOISE, (-20, -20, 300, 400), 2, 10)
			pygame.draw.rect(startScreen.display, CYBERGRAPE, (205, 200, 500, 500), 2, 10)
			pygame.draw.rect(startScreen.display, LIGHTGREEN, (-20, -20, 112, 600), 2, 10)


			# render the title
			startScreen.display.blit(title.display, title.rect)

			# render the buttons

			# render the log in button
			logInButton.render(mousePos)
			startScreen.display.blit(logInButton.surface.display, logInButton.surface.pos)

			# render the sign up button
			signUpButton.render(mousePos)
			startScreen.display.blit(signUpButton.surface.display, signUpButton.surface.pos)

			# render the screen
			self.window.display.blit(startScreen.display, startScreen.pos)

			# update
			pygame.display.update()

	def LogIn(self):
		""" Shows the log in screen for the game
		"""
		# create the log in screen
		logInScreen = Surface((0, 0), self.size, self.backgroundColor)
		

		# Create the header for the screen

		# create the header
		header = Surface((0, 0), (self.width, 50), CYBERGRAPE)

		# create the button to go back to the home page that doubles as the title
		titleButton = Button(Surface((header.width / 2 - 75, 2), (150, 50), CYBERGRAPE),
								Style(Text((150 / 2, 50 / 2), self.font, 25, "WORDLE", WHITE),
								fillColor=CYBERGRAPE))



		# Create the title for the screen
		title = Text((self.width / 2, 125), self.font, 75, "LOG IN", WHITE)

		# Create the Username label for the screen
		usernameLbl = Text((self.width / 4 + 20, 225), self.font, 30, "USERNAME", WHITE)

		# Create the username text box for the screen
		usernameTxt = TextBox(Surface((self.width / 4 - 30, 250), (300, 35), self.backgroundColor),
								Style(Text((10, 35 / 2), self.font, 20, "", WHITE),
                    			borderColor=TURQUOISE, borderRadius=5),

                   		 		selectedStyle=Style(borderColor=TURQUOISE, borderWidth=4, borderRadius=5))

		# Create the Password label for the screen
		passwordLbl = Text((self.width / 4 + 20, 325), self.font, 30, "PASSWORD", WHITE)

		# Create the password text box for the screen
		passwordTxt = TextBox(Surface((self.width / 4 - 30, 350), (300, 35), self.backgroundColor),
                        		Style(Text((10, 35 / 2), self.font, 20, "", WHITE),
								borderColor=TURQUOISE, borderRadius=5),

                        		selectedStyle=Style(borderColor=TURQUOISE, borderWidth=4, borderRadius=5))

		# Create the login button for the screen
		logInButton = Button(Surface((self.width / 2 + 20, 415), (150, 50), self.backgroundColor),
                       			Style(Text((75, 25), self.font, 25, "LOG IN", DENIM),
                                borderColor=DENIM, borderRadius=5),

								hoverStyle=Style(Text((75, 25), self.font, 26, "LOG IN", WHITE),
                         fillColor=DENIM, borderRadius=5))


		# Create the keyboard
		keyboard = self.createKeyboard(175, (5, 7))
		
		# create the potential alert message for the screen
		alert = Alert(Surface((-1, -1), (0, 0), self.backgroundColor), Text((0, 0), None, 0, "", (0, 0, 0)))

		while True:
			# run at 60 fps
			self.clock.tick(60)

			# clear the screen
			logInScreen.clear()

			# get the position of the mouse for later use
			mousePos = pygame.mouse.get_pos()

			# loop through the events
			for event in pygame.event.get():

				# Check for QUIT event
				if event.type == pygame.QUIT:
					return

				# Checks for the MOUSEDOWN event
				if event.type == pygame.MOUSEBUTTONDOWN:
					# if the mouse clicked the Title button go back to start
					if (titleButton.mouseIsHovering(mousePos)):
						self.Start()
						return

					# if the mouse clicked the log in button
					elif (logInButton.mouseIsHovering(mousePos)):
						
						verification = self.verifyLogin(usernameTxt.style.text.text, passwordTxt.style.text.text)
						if (verification[0]):
							self.Play()
							return
						# Show a message window
						alert = verification[1]

					elif (alert.mouseClickClose(mousePos)):
						alert = Alert(Surface((-1, -1), (0, 0), self.backgroundColor), Text((0, 0), None, 0, "", (0, 0, 0)))

					# check if the mouse hit the username text box
					usernameTxt.mouseClick(mousePos)

					# check if th mouse hit the password text box
					passwordTxt.mouseClick(mousePos)


				# Checks for the KEYDOWN event
				if event.type == pygame.KEYDOWN:
					# if the return key is pressed
					if event.key == pygame.K_RETURN:
						# jump to the password text box
						if (usernameTxt.isSelected):
							usernameTxt.isSelected = False
							passwordTxt.isSelected = True
						# attempt to log in
						elif(passwordTxt.isSelected):
							verification = self.verifyLogin(usernameTxt.style.text.text, passwordTxt.style.text.text)
							if (verification[0]):
								self.Play()
								return
							# Show a message window
							alert = verification[1]

					# if the tab key is pressed
					elif event.key == pygame.K_TAB:
						# jump to the password text box
						if (usernameTxt.isSelected):
							usernameTxt.isSelected = False
							passwordTxt.isSelected = True
						# jump to the username text box
						else:
							passwordTxt.isSelected = False
							usernameTxt.isSelected = True

					# if the key is the backspace
					elif event.key == pygame.K_BACKSPACE:
						# if username text box is selected -> backspace
						if (usernameTxt.isSelected):
							usernameTxt.backSpace()
						# if password text box is selected -> backspace
						elif (passwordTxt.isSelected):
							passwordTxt.backSpace()
					else:
						# if the username text box is selevted -> insert the character
						if (usernameTxt.isSelected):
							usernameTxt.insert(event.unicode)
						# if the password text box is selevted -> insert the character
						elif (passwordTxt.isSelected):
							passwordTxt.insert(event.unicode)
				


			# create the rectangles
			pygame.draw.rect(logInScreen.display, LIGHTGREEN, (-33, 69, 320, 700), 2, 10)
			pygame.draw.rect(logInScreen.display, ORCHID, (130, 200, 600, 600), 2, 10)
			pygame.draw.rect(logInScreen.display, CYBERGRAPE, (-33, 500, 600, 300), 2, 10)

			# render the header
			titleButton.render()
			header.display.blit(titleButton.surface.display, titleButton.surface.pos)
			logInScreen.display.blit(header.display, header.pos)

			# render the title
			logInScreen.display.blit(title.display, title.rect)

			# render the username label
			logInScreen.display.blit(usernameLbl.display, usernameLbl.rect)

			# render the username Text box
			usernameTxt.render()
			logInScreen.display.blit(usernameTxt.surface.display, usernameTxt.surface.pos)

			# render the password label
			logInScreen.display.blit(passwordLbl.display, passwordLbl.rect)
			
			# render the password Text Box
			passwordTxt.render()
			logInScreen.display.blit(passwordTxt.surface.display, passwordTxt.surface.pos)

			# render the login button
			logInButton.render(mousePos)
			logInScreen.display.blit(logInButton.surface.display, logInButton.surface.pos)

			# render the keyboard
			for key in keyboard:
				keyboard[key].render(mousePos)
				logInScreen.display.blit(keyboard[key].surface.display, keyboard[key].surface.pos)
			


			# render the alert
			alert.render(mousePos)
			logInScreen.display.blit(alert.surface.display, alert.surface.pos)

			# render the screen
			self.window.display.blit(logInScreen.display, logInScreen.pos)

			# update
			pygame.display.update()
		
	def SignUp(self):
		""" Shows the sign up screen for the game
		"""
		# create the log in screen
		signUpScreen = Surface((0, 0), self.size, self.backgroundColor)
		
		# Create the header for the screen

		# create the header
		header = Surface((0, 0), (self.width, 50), CYBERGRAPE)

		# create the button to go back to the home page that doubles as the title
		titleButton = Button(Surface((header.width / 2 - 75, 2), (150, 50), CYBERGRAPE),
								Style(Text((150 / 2, 50 / 2), self.font, 25, "WORDLE", WHITE),
								fillColor=CYBERGRAPE))



		# Create the title for the screen
		title = Text((self.width / 2, 100), self.font, 75, "SIGN UP", WHITE)



		# Create the Username label for the screen
		usernameLbl = Text((self.width / 4 + 20, 180), self.font, 25, "USERNAME", WHITE)

		# Create the username text box for the screen
		usernameTxt = TextBox(Surface((self.width / 4 - 30, 200), (300, 30), self.backgroundColor),
								Style(Text((10, 30 / 2), self.font, 15, "", WHITE),
                    			borderColor=LIGHTGREEN, borderRadius=5),

                   		 		selectedStyle=Style(borderColor=LIGHTGREEN, borderWidth=4, borderRadius=5))




		# Create the Password label for the screen
		passwordLbl = Text((self.width / 4 + 20, 270), self.font, 25, "PASSWORD", WHITE)

		# Create the password text box for the screen
		passwordTxt = TextBox(Surface((self.width / 4 - 30, 290), (300, 30), self.backgroundColor),
                        		Style(Text((10, 30 / 2), self.font, 15, "", WHITE),
                                borderColor=LIGHTGREEN, borderRadius=5),

                        		selectedStyle=Style(borderColor=LIGHTGREEN, borderWidth=4, borderRadius=5))

		# Create the verify Password label for the screen
		verifyPasswordLbl = Text((self.width / 4 + 70, 360), self.font, 25, "VERIFY PASSWORD", WHITE)

		# Create the verify password text box for the screen
		verifyPasswordTxt = TextBox(Surface((self.width / 4 - 30, 380), (300, 30), self.backgroundColor),
                        		Style(Text((10, 30 / 2), self.font, 15, "", WHITE),
                                borderColor=LIGHTGREEN, borderRadius=5),

                       			selectedStyle=Style(borderColor=LIGHTGREEN, borderWidth=4, borderRadius=5))

		# Create the login button for the screen
		logInButton = Button(Surface((self.width / 2 + 20, 440), (150, 50), self.backgroundColor),
                       			Style(Text((75, 25), self.font, 25, "LOG IN", ORCHID),
                                borderColor=ORCHID, borderRadius=5),

								hoverStyle=Style(Text((75, 25), self.font, 26, "LOG IN", WHITE),
                         		fillColor=ORCHID, borderRadius=5))

	 	# create the potential alert message for the screen
		alert = Alert(Surface((-1, -1), (0, 0), self.backgroundColor), Text((0, 0), None, 0, "", (0, 0, 0)))

		while True:
			# run at 60 fps
			self.clock.tick(60)

			# clear the screen
			signUpScreen.clear()

			# get the position of the mouse for later use
			mousePos = pygame.mouse.get_pos()

			# loop through the events
			for event in pygame.event.get():

				# Check for QUIT event
				if event.type == pygame.QUIT:
					return

				# Checks for the MOUSEDOWN event
				if event.type == pygame.MOUSEBUTTONDOWN:
					# if the mouse clicked the log in button
					if (titleButton.mouseIsHovering(mousePos)):
						self.Start()
						return
					elif (logInButton.mouseIsHovering(mousePos)):

						newUserSuccess = self.createNewUser(usernameTxt.style.text.text, passwordTxt.style.text.text, verifyPasswordTxt.style.text.text)
						if (newUserSuccess[0]):
							self.Play()
							return
						# Show a message window
						alert = newUserSuccess[1]

					# if the mouse clicks the close button on the alert
					elif (alert.mouseClickClose(mousePos)):
						alert = Alert(Surface((-1, -1), (0, 0), self.backgroundColor), Text((0, 0), None, 0, "", (0, 0, 0)))


					# check if the mouse hit the username text box
					usernameTxt.mouseClick(mousePos)

					# check if th mouse hit the password text box
					passwordTxt.mouseClick(mousePos)

					# check if the mouse hit the verify password text box
					verifyPasswordTxt.mouseClick(mousePos)


				# Checks for the KEYDOWN event
				if event.type == pygame.KEYDOWN:
					# if the return key is pressed
					if event.key == pygame.K_RETURN:
						# jump to the password text box
						if (usernameTxt.isSelected):
							usernameTxt.isSelected = False
							verifyPasswordTxt.isSelected = False
							passwordTxt.isSelected = True
						# jump to the verify password text box
						elif(passwordTxt.isSelected):
							passwordTxt.isSelected = False
							verifyPasswordTxt.isSelected = True
						# Sign up
						else:
							newUserSuccess = self.createNewUser(usernameTxt.style.text.text, passwordTxt.style.text.text, verifyPasswordTxt.style.text.text)
							if (newUserSuccess[0]):
								self.Play()
								return
							# Show a message window
							alert = newUserSuccess[1]
							
					# if the tab key is pressed
					elif event.key == pygame.K_TAB:
						# jump to the password text box
						if (usernameTxt.isSelected):
							usernameTxt.isSelected = False
							passwordTxt.isSelected = True
						elif (passwordTxt.isSelected):
							usernameTxt.isSelected = False
							passwordTxt.isSelected = False
							verifyPasswordTxt.isSelected = True
						# jump to the username text box
						else:
							verifyPasswordTxt.isSelected = False
							passwordTxt.isSelected = False
							usernameTxt.isSelected = True

					# if the key is the backspace
					elif event.key == pygame.K_BACKSPACE:
						# if username text box is selected -> backspace
						if (usernameTxt.isSelected):
							usernameTxt.backSpace()
						# if password text box is selected -> backspace
						elif (passwordTxt.isSelected):
							passwordTxt.backSpace()
						# if verify password text box is selected -> backspace
						elif (verifyPasswordTxt.isSelected):
							verifyPasswordTxt.backSpace()
					else:
						# if the username text box is selevted -> insert the character
						if (usernameTxt.isSelected):
							usernameTxt.insert(event.unicode)
						# if the password text box is selevted -> insert the character
						elif (passwordTxt.isSelected):
							passwordTxt.insert(event.unicode)
						# if verify password text box is selected -> insert the character
						elif (verifyPasswordTxt.isSelected):
							verifyPasswordTxt.insert(event.unicode)
				


			# create the rectangles
			pygame.draw.rect(signUpScreen.display, TURQUOISE, (-33, 69, 320, 700), 2, 10)
			pygame.draw.rect(signUpScreen.display, DENIM, (130, 200, 600, 600), 2, 10)
			pygame.draw.rect(signUpScreen.display, CYBERGRAPE, (-33, 500, 600, 300), 2, 10)

			# render the header
			titleButton.render()
			header.display.blit(titleButton.surface.display, titleButton.surface.pos)
			signUpScreen.display.blit(header.display, header.pos)

			# render the title
			signUpScreen.display.blit(title.display, title.rect)

			# render the username label
			signUpScreen.display.blit(usernameLbl.display, usernameLbl.rect)

			# render the username Text box
			usernameTxt.render()
			signUpScreen.display.blit(usernameTxt.surface.display, usernameTxt.surface.pos)

			# render the password label
			signUpScreen.display.blit(passwordLbl.display, passwordLbl.rect)

			# render the password Text Box
			passwordTxt.render()
			signUpScreen.display.blit(passwordTxt.surface.display, passwordTxt.surface.pos)

			# render the password label
			signUpScreen.display.blit(verifyPasswordLbl.display, verifyPasswordLbl.rect)

			# render the password Text Box
			verifyPasswordTxt.render()
			signUpScreen.display.blit(verifyPasswordTxt.surface.display, verifyPasswordTxt.surface.pos)

			# render the login button
			logInButton.render(mousePos)
			signUpScreen.display.blit(logInButton.surface.display, logInButton.surface.pos)

			# render the alert
			alert.render(mousePos)
			signUpScreen.display.blit(alert.surface.display, alert.surface.pos)

			# render the screen
			self.window.display.blit(signUpScreen.display, signUpScreen.pos)

			# update
			pygame.display.update()

	def Play(self):
		"""Plays the game"""
		# create the game in screen
		gameScreen = Surface((0, 0), self.size, self.backgroundColor)
		gameover = False
		win = False

		# Create the header for the screen

		# create the header
		header = Surface((0, 0), (self.width, 50), CYBERGRAPE)

		# create the button to go back to the home page that doubles as the title
		titleButton = Button(Surface((header.width / 2 - 75, 2), (150, 50), CYBERGRAPE),
                       Style(Text((150 / 2, 50 / 2), self.font, 25, "WORDLE", WHITE),
                             fillColor=CYBERGRAPE))

		# the words for the game
		secretWord = random.choice(self.acceptedWordsList).upper()
		words = [""] * 6
		currentWord = 0
		greenLetters = ""
		yellowLetters = ""
		blackLetters = ""

		keyboard = self.createKeyboard(175, (5, 7))


		alert = Alert(Surface((-1, -1), (0, 0), self.backgroundColor), Text((0, 0), None, 0, "", (0, 0, 0)))

		notAWordAlert = Alert(Surface((-1, -1), (0, 0),
		              self.backgroundColor), Text((0, 0), None, 0, "", (0, 0, 0)))

		displayAlertTime = pygame.time.get_ticks()

		endScreen = Surface((self.width / 2 - 150, self.height / 2 - 200), (300, 400), self.backgroundColor)
		# display the play agian button
		playAgainBtn = Button(Surface((10, 340), (125, 50), self.backgroundColor),
								Style(Text((125 / 2, 25), self.font, 15, "PLAY AGAIN", DENIM), borderColor=DENIM, borderRadius=5),
								hoverStyle=Style(Text((125 / 2, 25), self.font, 17, "PLAY AGAIN", WHITE), fillColor=DENIM, borderRadius=5))
		

		# display the exit button
		exitBtn = Button(Surface((165, 340), (125, 50), self.backgroundColor),
								Style(Text((125 / 2, 25), self.font, 15, "EXIT", LIGHTRED), borderColor=LIGHTRED, borderRadius=5),
								hoverStyle=Style(Text((125 / 2, 25), self.font, 17, "EXIT", WHITE), fillColor=LIGHTRED, borderRadius=5))
		

		while True:
			# run at 60 fps
			self.clock.tick(60)


			# check for game over
			if (not gameover and words[currentWord - 1] == secretWord):
				pygame.time.delay(2000)
				gameover = True
				win = True
			elif (not gameover and currentWord == 6):
				pygame.time.delay(2000)
				gameover = True


			

			# clear the screen
			gameScreen.clear()

			# get the position of the mouse for later use
			mousePos = pygame.mouse.get_pos()

			# loop through the events
			for event in pygame.event.get():

				# Check for QUIT event
				if event.type == pygame.QUIT:
					return

				# Checks for the MOUSEDOWN event
				if event.type == pygame.MOUSEBUTTONDOWN:
					# if the mouse clicked the Title button go back to start
					if (titleButton.mouseIsHovering(mousePos)):
						self.Start()
						return

					# if the mouse clicked the close button on the alert
					elif (alert.mouseClickClose(mousePos)):
						alert = Alert(Surface((-1, -1), (0, 0),
						              self.backgroundColor), Text((0, 0), None, 0, "", (0, 0, 0)))
					
					# Check to see if the mouse clicked on the keyboard
					elif (not gameover):
						# loop throught the keys and check to see if the mouse clicked them
						for key in keyboard:
							if (keyboard[key].mouseIsHovering(mousePos)):
								# if the key is the enter button encrement the current word
								if (key == "ENTER"):
									if (len(words[currentWord]) == 5):
										if (words[currentWord].lower() in self.acceptedWords):
											currentWord += 1
										else:
											notAWordAlert = Alert(Surface((self.width / 2 - 125, 40), (250, 75), self.backgroundColor), Text((125, 75 / 2), self.font, 25, "Not in Word List", BLACK))
											notAWordAlert.closeButton = Button(Surface((0, 0), (0, 0), self.backgroundColor), Style())
											displayAlertTime = pygame.time.get_ticks()
								# if the key is the back button delete the last character of the current word
								elif (key == "BACK"):
									words[currentWord] = words[currentWord][:-1]
								# else just add the word if the length is less than 5
								elif (len(words[currentWord]) < 5):
									words[currentWord] += key

					# Check to see if the mouse clicked the play again button
					elif (playAgainBtn.mouseIsHovering((mousePos[0] - endScreen.pos[0], mousePos[1] - endScreen.pos[1]))):
						self.Play()
						return
					
					# Check to see if the mouse clicked the eixt button
					elif (exitBtn.mouseIsHovering((mousePos[0] - endScreen.pos[0], mousePos[1] - endScreen.pos[1]))):
						self.Start()
						return

				# Checks for the KEYDOWN event
				if event.type == pygame.KEYDOWN and not gameover:
					# if the return key is pressed
					if event.key == pygame.K_RETURN:
						if (len(words[currentWord]) == 5):
							if (words[currentWord].lower() in self.acceptedWords):
								currentWord += 1
							else:
								notAWordAlert = Alert(Surface((self.width / 2 - 125, 40), (250, 75), self.backgroundColor), Text((125, 75 / 2), self.font, 25, "Not in Word List", BLACK))
								notAWordAlert.closeButton = Button(Surface((0, 0), (0, 0), self.backgroundColor), Style())
								displayAlertTime = pygame.time.get_ticks()
					# if the backspace key is pressed
					elif event.key == pygame.K_BACKSPACE:
						words[currentWord] = words[currentWord][:-1]
					# else just add to the word if the length is less than 5
					elif (len(words[currentWord]) < 5):
						words[currentWord] += event.unicode.upper()

			# create the aesthetic lines
			pygame.draw.rect(gameScreen.display, LIGHTGREEN,
			                 (-33, 69, 320, 700), 2, 10)
			pygame.draw.rect(gameScreen.display, ORCHID, 
							 (130, 200, 600, 600), 2, 10)
			pygame.draw.rect(gameScreen.display, CYBERGRAPE,
			                 (-33, 500, 600, 300), 2, 10)
							 

			# render the header
			titleButton.render()
			header.display.blit(titleButton.surface.display, titleButton.surface.pos)
			gameScreen.display.blit(header.display, header.pos)


			# Create the word boxes
			wordsHeight = 400
			padding = 40
			margin = 8
			boxSize = (wordsHeight - padding * 2 - margin * 4) / 5
			for i in range(0, len(words)):
				# make sure the word has been entered
				if (i < currentWord):

					# created a copy of the words
					text = words[i]
					copyOfSecretWord = secretWord
					word = {}
					for j in range(0, 5):
						word.update({j: text[j]})
					
					# loop through and check to see if we should turn any letters green
					for j in word.copy():
						# if the letters are in the correct spot
						if word[j] == copyOfSecretWord[j]:
							# add it to greenletters
							if (text[j] not in greenLetters):
								greenLetters += text[j]

							# create the box
							box = Button(Surface((padding + j * (boxSize + margin), i * (boxSize + margin) + 75), (boxSize, boxSize), self.backgroundColor), Style(Text(
								(boxSize / 2, boxSize / 2), self.font, 25, text[j], WHITE), borderColor=WHITE, borderRadius=5, fillColor=GREEN))
							box.render()
							gameScreen.display.blit(box.surface.display, box.surface.pos)

							# delete the letter from the copy of the words
							copyOfSecretWord = copyOfSecretWord[:j] + " " + copyOfSecretWord[j + 1:]
							word.pop(j)
					
					# loop throuhg and check to see if we should turn any yellow
					for j in word.copy():
						fillcolor = DARKGRAY

						# if the letters are in the word
						if (word[j] in copyOfSecretWord):
							# add it to yellowletters
							if (text[j] not in yellowLetters):
								yellowLetters += text[j]
							
							# change the fillcolor
							fillcolor = YELLOW

							# delete the letter from the copy of the words
							index = copyOfSecretWord.index(word[j])
							copyOfSecretWord = copyOfSecretWord[:index] + " " + copyOfSecretWord[index + 1:]
							word.pop(j)
						# else the letters are not in the word
						elif (text[j] not in blackLetters):
							blackLetters += text[j]
						
						# render the box
						box = Button(Surface((padding + j * (boxSize + margin), i * (boxSize + margin) + 75), (boxSize, boxSize), self.backgroundColor), Style(Text(
								(boxSize / 2, boxSize / 2), self.font, 25, text[j], WHITE), borderColor=WHITE, borderRadius=5, fillColor=fillcolor))
						box.render()
						gameScreen.display.blit(box.surface.display, box.surface.pos)

				else:
					textColor = WHITE
					if i == currentWord and words[i] not in self.acceptedWords:
						textColor = WHITE


					for j in range(0, 5):
						# get the text from the words
						text = words[i][j] if j < len(words[i]) else ""

						# render the box
						box = Button(Surface((padding + j * (boxSize + margin), i * (boxSize + margin) + 75), (boxSize, boxSize), self.backgroundColor), Style(Text(
								(boxSize / 2, boxSize / 2), self.font, 25, text, WHITE), borderColor=WHITE, borderRadius=5, fillColor=self.backgroundColor))
						box.render()
						gameScreen.display.blit(box.surface.display, box.surface.pos)

			

			# render the keyboard
			for key in keyboard:
				if (key in greenLetters):
					keyboard[key].style.fillColor = GREEN
					keyboard[key].hoverStyle.fillColor = DARKGREEN
				elif (key in yellowLetters):
					keyboard[key].style.fillColor = YELLOW
					keyboard[key].hoverStyle.fillColor = DARKYELLOW
				elif (key in blackLetters):
					keyboard[key].style.fillColor = DARKGRAY
					keyboard[key].hoverStyle.fillColor = DARKERGRAY

				
				keyboard[key].render(mousePos)
				gameScreen.display.blit(keyboard[key].surface.display, keyboard[key].surface.pos)
			

			# display the end screen if game over
			if (gameover):
				endScreen.clear()

				# display the title
				if (win):
					text = Text((endScreen.width / 2, 50), self.font, 50, "VICTORY!", LIGHTGREEN)
				else:
					text = Text((endScreen.width / 2, 50), self.font, 50, "DEFEAT", (255, 0, 0))
					word = Text((endScreen.width / 2, 80), self.font, 15, f"The word was {secretWord}", GRAY)
					endScreen.display.blit(word.display, word.rect)
				endScreen.display.blit(text.display, text.rect)


				# render play again button
				playAgainBtn.render((mousePos[0] - endScreen.pos[0], mousePos[1] - endScreen.pos[1]))
				endScreen.display.blit(playAgainBtn.surface.display, playAgainBtn.surface.pos)

				# render exit button
				exitBtn.render((mousePos[0] - endScreen.pos[0], mousePos[1] - endScreen.pos[1]))
				endScreen.display.blit(exitBtn.surface.display, exitBtn.surface.pos)
				

				# create a layover to hide the other screen
				layOver = pygame.Surface((self.width, self.height), pygame.SRCALPHA)
				layOver.fill((0, 0, 0, 128))
				gameScreen.display.blit(layOver, (0,0))
				gameScreen.display.blit(endScreen.display, endScreen.pos)

			# render the alert
			alert.render(mousePos)
			gameScreen.display.blit(alert.surface.display, alert.surface.pos)

			if (pygame.time.get_ticks() - displayAlertTime < 2000):
				# render the not a word alert
				notAWordAlert.render(mousePos)
				gameScreen.display.blit(notAWordAlert.surface.display, notAWordAlert.surface.pos)


			# render the screen
			self.window.display.blit(gameScreen.display, gameScreen.pos)

			# update
			pygame.display.update()
		

	def verifyLogin(self, username: str, password: str) -> Tuple[bool, Alert]:
		"""Verifies the login username and password

		Args:
			username (str): The username to compare the password to
			password (str): The password for the username

		Returns:
			Tuple[bool, Alert]: [0] whether it was a success or not, [1] the alert to display on fail
		"""
		
		error = ""
		# If the user didn't fill in the username field
		if username.strip() == "":
			error = "USERNAME is a required field."
		# If the user didn't fill in the password field
		elif password.strip() == "":
			error = "PASSWORD is a required field."
		# If the login is invalid
		elif False:
			# Verify that the login has correct username and password
			pass

		# Else the verification was successful
		else:							
			return [True, Alert(Surface((self.width / 2 - 350 / 2, 80), (350, 100), self.backgroundColor),
                      Text((145, 50), self.font, 18, "Success!", BLACK), "Success")]

		return [False, Alert(Surface((self.width / 2 - 350 / 2, 80), (350, 100), self.backgroundColor),
									 Text((145, 50), self.font, 18, error, BLACK), "Warning")]

	def createNewUser(self, username: str, password: str, verifyPassword: str) -> Tuple[bool, Alert]:
		"""Creates a new user with the given username and password

		Args:
			username (str): the username for the user
			password (str): The password for the user
			verifyPassword (str): The password verification for the user

		Returns:
			Tuple[bool, Alert]: [0] whether it was a success or not, [1] the alert to display on fail
		"""
		error = ""
		type = "Warning"
		fontSize = 18
		# If the user didn't fill in the username field
		if username.strip() == "":
			error = "USERNAME is a required field."
		# If the user didn't fill in the password field
		elif password.strip() == "":
			error = "PASSWORD is a required field."
		# If the user didn't fill in the verify password field
		elif verifyPassword.strip() == "":
			error = "VERIFY PASSWORD is a required field."
			fontSize = 14
		# Check to make sure the passwords match
		elif password != verifyPassword:
			error = "PASSWORDS don't match."
		# If the login is invalid
		elif False:
			# Verify that the login has correct username and password
			type = "Danger"

		# After all the validation return true
		else:
			return [True, None]

		return[False, Alert(Surface((self.width / 2 - 350 / 2, 80), (350, 100), self.backgroundColor),
									Text((145, 50), self.font, fontSize, error, BLACK), type)]

	def createKeyboard(self, keyboardHeight: int, margin: Tuple[int, int]) -> dict[str, Button]:
		"""Returns a keyboard with the specified hieght at the bottom of the screen

		Args:
			keyBoardHeight (int): the height of the keyboard
			margin (Tuple[int, int]): The space between the keys. margin[0] is x and margin[1] is y.

		Returns:
			Dict(str, Button): The keyboard. Dict[letter] = button
		"""

		keyboard = {}

		keyHeight = (keyboardHeight - margin[1] * 3) / 3
		keyWidth = (self.width - margin[0] * 11) / 10


		qRow = "QWERTYUIOP"
		for i, letter in enumerate(qRow):
			keyboard.update({letter: Button(Surface((i * keyWidth + (i + 1) * margin[0], self.height - keyboardHeight), (keyWidth, keyHeight), self.backgroundColor),
						Style(Text((keyWidth / 2, keyHeight / 2), self.font, 14, letter, WHITE), borderRadius=3, fillColor=LIGHTGRAY),
						hoverStyle = Style(Text((keyWidth / 2, keyHeight / 2), self.font, 15, letter, WHITE), borderRadius=3, fillColor=GRAY))})

		aRow = "ASDFGHJKL"
		for i, letter in enumerate(aRow):
			keyboard.update({letter: Button(Surface((i * keyWidth + ((i + 1) * margin[0] + margin[0] / 2) + (keyWidth / 2), self.height - keyboardHeight * 2 / 3), (keyWidth, keyHeight), self.backgroundColor),
						Style(Text((keyWidth / 2, keyHeight / 2), self.font, 14, letter, WHITE), borderRadius=3, fillColor=LIGHTGRAY),
						hoverStyle = Style(Text((keyWidth / 2, keyHeight / 2), self.font, 15, letter, WHITE), borderRadius=3, fillColor=GRAY))})

		zRow = "ZXCVBNM"
		keyboard.update({"ENTER": Button(Surface((margin[0] + 2, self.height - keyboardHeight / 3), (keyWidth * 3 / 2, keyHeight), self.backgroundColor),
						Style(Text((keyWidth * 3 / 4, keyHeight / 2), self.font, 12, "ENTER", WHITE), borderRadius=3, fillColor=LIGHTGRAY),
						hoverStyle = Style(Text((keyWidth * 3 / 4, keyHeight / 2), self.font, 13, "ENTER", WHITE), borderRadius=3, fillColor=GRAY))})
		for i, letter in enumerate(zRow):
			keyboard.update({letter: Button(Surface((i * keyWidth + ((i + 1) * margin[0] + margin[0] * 3 / 2) + keyWidth * 3 / 2, self.height - keyboardHeight / 3), (keyWidth, keyHeight), self.backgroundColor),
						Style(Text((keyWidth / 2, keyHeight / 2), self.font, 14, letter, WHITE), borderRadius=3, fillColor=LIGHTGRAY),
						hoverStyle = Style(Text((keyWidth / 2, keyHeight / 2), self.font, 15, letter, WHITE), borderRadius=3, fillColor=GRAY))})

		keyboard.update({"BACK": Button(Surface((keyWidth * 17 / 2 + margin[0] * 9 + 3, self.height - keyboardHeight / 3), (keyWidth * 3 / 2, keyHeight), self.backgroundColor),
					Style(Text((keyWidth * 3 / 4, keyHeight / 2), self.font, 12, "BACK", WHITE), borderRadius=3, fillColor=LIGHTGRAY),
					hoverStyle = Style(Text((keyWidth * 3 / 4, keyHeight / 2), self.font, 13, "BACK", WHITE), borderRadius=3, fillColor=GRAY))})

		return keyboard


		
wordle = Wordle()
wordle.Play()